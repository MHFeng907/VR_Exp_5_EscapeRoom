using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

public class Canvas_AI_VR : MonoBehaviour
{
    [Header("UI 分组")]
    public GameObject helperUIGroup;
    public TMP_InputField questionInput;
    public GameObject xrKeyboard;
    public GameObject answerPanel;
    public TextMeshProUGUI answerText;

    [Header("API 配置")]
    public string apiUrl = "https://api.deepseek.com/v1/chat/completions";
    public string apiKey = "your-api-key-here";

    private bool isVisible = false;

    void Start()
    {
        Debug.Log("[Init] 初始化UI状态");
        helperUIGroup.SetActive(false);
        xrKeyboard.SetActive(false);
        answerPanel.SetActive(false);
        
        if (questionInput != null)
        {
            questionInput.gameObject.SetActive(false);
            questionInput.interactable = true;
        }
    }

    public void ToggleHelper()
    {
        Debug.Log($"[UI] 切换助手显示状态，当前: {isVisible} -> {!isVisible}");
        isVisible = !isVisible;

        if (isVisible)
        {
            helperUIGroup.SetActive(true);
            
            if (questionInput != null)
            {
                questionInput.gameObject.SetActive(true);
                questionInput.interactable = true;
                questionInput.text = "";
                StartCoroutine(DelayedActivateInputField());
            }

            xrKeyboard.SetActive(true);
            answerPanel.SetActive(false);
        }
        else
        {
            helperUIGroup.SetActive(false);
            xrKeyboard.SetActive(false);
        }
    }

    private IEnumerator DelayedActivateInputField()
    {
        yield return null;
        Debug.Log("[UI] 激活输入框");
        questionInput.ActivateInputField();
    }

    public void OnSubmit()
    {
        if (questionInput == null)
        {
            Debug.LogError("[Submit] 错误: questionInput 未赋值!");
            return;
        }

        string question = questionInput.text.Trim();
        Debug.Log($"[Submit] 提交问题: '{question}'");

        if (string.IsNullOrEmpty(question))
        {
            Debug.LogWarning("[Submit] 问题为空");
            answerText.text = "问题不能为空。";
            answerPanel.SetActive(true);
            return;
        }

        questionInput.DeactivateInputField();
        xrKeyboard.SetActive(false);
        answerText.text = "思考中...";
        answerPanel.SetActive(true);

        Debug.Log("[API] 开始发送请求...");
        StartCoroutine(SendRequest(question));
    }

    private IEnumerator SendRequest(string question)
    {
        // 构造请求数据
        string jsonPayload = $@"{{
            ""model"": ""deepseek-chat"",
            ""messages"": [{{""role"":""user"", ""content"":""{EscapeJson(question)}""}}]
        }}";

        Debug.Log($"[API] 请求数据:\n{jsonPayload}");

        using (UnityWebRequest req = new UnityWebRequest(apiUrl, "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonPayload));
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            req.SetRequestHeader("Authorization", $"Bearer {apiKey}");

            Debug.Log("[API] 发送请求...");
            yield return req.SendWebRequest();

            Debug.Log($"[API] 请求完成. 状态: {req.result}");

            yield return new WaitForSeconds(1f);
            
            if (req.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"[API] 响应成功:\n{req.downloadHandler.text}");
                ProcessResponse(req.downloadHandler.text);
            }
            else
            {
                Debug.LogError($"[API] 请求失败: {req.error}\n响应: {req.downloadHandler?.text}");
                answerText.text = $"请求失败：{req.error}";
            }
        }
    }

    private void ProcessResponse(string json)
    {
        Debug.Log($"[API] 开始解析响应: {json}");
        //answerPanel.SetActive(true);
        try
        {
            var data = JsonUtility.FromJson<DeepSeekResponse>(json);
            string responseContent = data?.choices?[0]?.message?.content;
            
            if (!string.IsNullOrEmpty(responseContent))
            {
                Debug.Log($"[API] 解析成功，内容: {responseContent}");
                answerPanel.SetActive(true);
                answerText.text = responseContent;
                Debug.Log($"[UI] 已设置 answerText：{answerText.text}");
            }
            else
            {
                Debug.LogWarning("[API] 解析成功但内容为空");
                answerText.text = "未收到有效回答。";
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[API] 解析异常: {e.Message}");
            answerText.text = "解析失败。";
        }
    }

    private string EscapeJson(string input)
    {
        return input.Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\t", "\\t");
    }

    [System.Serializable] 
    private class DeepSeekResponse { public Choice[] choices; }
    [System.Serializable] 
    private class Choice { public Message message; }
    [System.Serializable] 
    private class Message { public string content; }
}