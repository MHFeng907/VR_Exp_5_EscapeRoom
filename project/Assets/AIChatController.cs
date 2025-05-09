using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Collections;

public class AIChatController : MonoBehaviour
{
    [Header("UI Settings")]
    public TMP_InputField questionInput;  // 提问输入框
    public TMP_InputField answerInput;    // 回答输入框

    [Header("API Configuration")]
    public string apiUrl = "https://api.deepseek.com/v1/chat/completions";
    public string apiKey = "your-api-key-here";

    private bool chatUIVisible = false;   // 是否显示提问+回答UI
    private bool inputActive = false;     // 是否已经进入输入问题状态

    private void Start()
    {
        SetChatUIActive(false); // 初始隐藏
    }

    private void Update()
    {
        // Ctrl + I 打开聊天界面
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.I))
        {
            SetChatUIActive(true);
            inputActive = false; // 初始不允许打字
            Debug.Log("[Input] Ctrl+I 打开聊天界面");
        }

        // Ctrl + E 隐藏聊天界面
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.E))
        {
            SetChatUIActive(false);
            inputActive = false;
            Debug.Log("[Input] Ctrl+E 隐藏聊天界面");
        }

        if (chatUIVisible)
        {
            // 空格键激活输入模式
            if (!inputActive && Input.GetKeyDown(KeyCode.Space))
            {
                inputActive = true;
                questionInput.ActivateInputField();
                Debug.Log("[Input] 按下空格，开始接受键盘输入");
            }

            // 在输入模式下，按回车提交问题
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log("[Input] 按下回车，提交问题");
                OnSubmitQuestion();
                inputActive = false; // 重置输入状态，需要再次按空格才能继续输入下一个问题
            }
        }
    }

    private void SetChatUIActive(bool active)
    {
        if (questionInput != null) questionInput.gameObject.SetActive(active);
        if (answerInput != null) answerInput.gameObject.SetActive(active);

        if (!active)
        {
            if (questionInput != null) questionInput.text = "";
            if (answerInput != null) answerInput.text = "";
        }

        chatUIVisible = active;
    }

    public void OnSubmitQuestion()
    {
        if (string.IsNullOrEmpty(questionInput.text))
        {
            Debug.LogWarning("[Input] 提交了空问题！");
            answerInput.text = "问题不能为空！";
            return;
        }

        Debug.Log($"[API] 提交问题：{questionInput.text}");
        StartCoroutine(SendAPIRequest(questionInput.text));
        answerInput.text = "思考中...";
        questionInput.text = "";
    }

    private IEnumerator SendAPIRequest(string question)
    {
        string jsonPayload = $@"{{
            ""model"": ""deepseek-chat"",
            ""messages"": [
                {{
                    ""role"": ""user"",
                    ""content"": ""{EscapeJsonString(question)}""
                }}
            ]
        }}";

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] payloadBytes = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(payloadBytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {apiKey}");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                ProcessSuccessfulResponse(request.downloadHandler.text);
            }
            else
            {
                HandleRequestError(request);
            }
        }
    }

    private void ProcessSuccessfulResponse(string jsonResponse)
    {
        try
        {
            DeepSeekResponse responseData = JsonUtility.FromJson<DeepSeekResponse>(jsonResponse);

            if (responseData.choices != null && responseData.choices.Length > 0)
            {
                string answer = responseData.choices[0].message.content;
                Debug.Log($"[API] 回答内容：{answer}");
                answerInput.text = answer;
            }
            else
            {
                Debug.LogWarning("[API] 没有收到有效回答！");
                answerInput.text = "未收到有效回答。";
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[API] JSON解析失败: {e.Message}");
            answerInput.text = "回答解析失败。";
        }
    }

    private void HandleRequestError(UnityWebRequest request)
    {
        string errorMessage = $"[API] 请求失败：{request.error}\n响应：{request.downloadHandler?.text ?? "无"}";
        Debug.LogError(errorMessage);
        answerInput.text = "请求失败。";
    }

    private string EscapeJsonString(string input)
    {
        return input.Replace("\"", "\\\"")
                   .Replace("\n", "\\n")
                   .Replace("\t", "\\t");
    }

    [System.Serializable]
    private class DeepSeekResponse
    {
        public Choice[] choices;
    }

    [System.Serializable]
    private class Choice
    {
        public Message message;
    }

    [System.Serializable]
    private class Message
    {
        public string content;
    }
}
