using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using UnityEngine.Events;
using HMSXR.Foundation;

namespace XvXR.Foundation
{
    public class result {
        public  string word;//命令词
        public int id;// id
        public int sc;//置信度
    }
   /// <summary>
   /// 提供语音识别的配置方法以及识别接口
   /// </summary>
    public sealed class XvSpeechVoiceManager : MonoBehaviour
    {
        private XvSpeechVoiceManager() { }

        private const string ENGINE_TYPE = "local";

        private string LOCAL_BNF = "#BNF+IAT 1.0 UTF-8;\n"
        + "!grammar word;\n"
        + "!slot <words>;\n"
        + "!start <words>;\n";
        private const string LOCAL_GRAMMAR = "word";

        private const string LOCAL_THRESHOLD = "60";
        private UnityAction<string> onStatusRecognized;//所有的语音状态

        private result result1 = new result();

        public UnityAction<result> onCommandRecognized;
        /// <summary>
        /// 识别到的语音词命令 id  word
        /// </summary>
        [SerializeField]
        private List<AitalkWord> aitalkWords = new List<AitalkWord>();

        [Tooltip("置信度阈值")]
        [Range(0,100)]
        [SerializeField]
        private int sc=20;



        void Start()
        {
            this.name = "Aitalk";

            StringBuilder stringBuilder = new StringBuilder("<words>:");

            for (int i = 0; i < aitalkWords.Count; i++)
            {
                if (i == aitalkWords.Count - 1)
                {
                    
                    stringBuilder.Append(string.Format("{0}!id({1});\n", aitalkWords[i].word, aitalkWords[i].id));

                }
                else
                {
                    stringBuilder.Append(string.Format("{0}!id({1})|", aitalkWords[i].word, aitalkWords[i].id));

                }
            }

            LOCAL_BNF += stringBuilder.ToString();

            


            //LOCAL_BNF = "#BNF+IAT 1.0 UTF-8;\n" +
            //    "!grammar call;!slot <name>;\n" +
            //    "!start <commands>;\n" +
            //    "<commands>:(找一下|打电话给) <name>;\n" +
            //    "<name>: 张三|李四;";

           

            MyDebugTool.LogError(LOCAL_BNF);
#if UNITY_EDITOR
            return;
#endif

            Invoke("startASR", 3);
        }

      

        public void startASR()
        {

            try { 
            
            UpdateText("startASR");
            AndroidHelper.CallObjectMethod(InterfaceObject, "init", new object[] { });
            UpdateText("init");

            AndroidHelper.CallObjectMethod(InterfaceObject, "buildGrammar", new object[] { ENGINE_TYPE, LOCAL_BNF });
            UpdateText("buildGrammar");

            AndroidHelper.CallObjectMethod(InterfaceObject, "setParam", new object[] { ENGINE_TYPE, LOCAL_GRAMMAR, LOCAL_THRESHOLD });
            UpdateText("setParam");

            AndroidHelper.CallObjectMethod(InterfaceObject, "startASR", new object[] { });
            UpdateText("startASREnd");
            }catch (Exception e)
            {

                MyDebugTool.Log("aitak_log:Exception  " + e.Message);
            }

        }

        /*public void StartARS2()
        {
            Debug.LogError("========== StartARS");
        }*/


        private AndroidJavaObject interfaceObject;

        private AndroidJavaObject InterfaceObject
        {
            get
            {
                if (interfaceObject == null)
                {
                    AndroidJavaClass activityClass = XvAndroidHelper.GetClass("com.unity3d.player.UnityPlayer");
                    AndroidJavaObject activityObject = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
                    if (activityObject != null)
                    {
                        interfaceObject = XvAndroidHelper.Create("com.xv.aitalk.UnityInterface", new object[] { activityObject });
                    }
                }
                return interfaceObject;
            }
        }

        private void UpdateText(string text)
        {
           MyDebugTool.Log(text);
            //statusText.text = text;
        }

        public void onInit(string result)
        {
            onStatusRecognized?.Invoke(result);

            MyDebugTool.Log("aitak_log:unity:LogInfo:onInit:" + result);
            UpdateText("init" + result);
        }

        public void onBuildFinish(string result)
        {
            onStatusRecognized?.Invoke(result);

            MyDebugTool.Log("aitak_log:unity:LogInfo:onBuildFinish:" + result);
            string[] strArray = result.Split('|');
            if (strArray.Length > 1)
            {
                UpdateText("onBuildFinish:" + strArray[1]);
                onStatusRecognized?.Invoke("onInit");

            }
            else
            {
                UpdateText("onBuildFinish:false");
            }

        }

        public void onBeginOfSpeech(string nullstr)
        {
            MyDebugTool.Log("aitak_log:unity:LogInfo:onBeginOfSpeech.....");
            UpdateText("说话中....");

            onStatusRecognized?.Invoke(nullstr);
        }

        public void onEndOfSpeech(string nullstr)
        {
            MyDebugTool.Log("aitak_log:unity:LogInfo:onEndOfSpeech.....");
            UpdateText("说话结束");
            onStatusRecognized?.Invoke(nullstr);

        }

        public void onError(string error)
        {
            onStatusRecognized?.Invoke(error);

            Debug.LogError("aitak_log:unity:LogError:" + error);
            UpdateText(error);
        }


        public void onResult(string result)
        {
            onStatusRecognized?.Invoke(result);

            MyDebugTool.Log("aitak_log:unity:LogInfo:onResult:" + result);
            string[] strArray = result.Split('|');
            if (strArray.Length > 1)
            {

                if ("true" == strArray[1])
                {
                    try
                    {
                        XvAitalkModels.result data = SimpleJson.SimpleJson.DeserializeObject<XvAitalkModels.result>(result, new JsonSerializerStrategy());
                        if (data != null)
                        {
                            if (data.sc > sc)
                            {
                                for (int i = 0; i < aitalkWords.Count; i++)
                                {
                                    if (aitalkWords[i].id == data.ws[0].cw[0].id)
                                    {
                                        aitalkWords[i].action?.Invoke();

                                        result1.id = data.ws[0].cw[0].id;
                                        result1.word = aitalkWords[i].word;
                                        result1.sc = data.sc;

                                        onCommandRecognized?.Invoke(result1);
                                        break;
                                    }
                                }
                            }
                            //UpdateText("识别结果:true，可信度:" + data.sc + ",内容:" + data.ws[0].cw[0].w + ",id:" + data.ws[0].cw[0].id);
                        }
                    }
                    catch (Exception e)
                    {
                        UpdateText("onResult:false");
                    }
                }
                else
                {
                    UpdateText("onResult:false");
                }
            }
            else
            {
                UpdateText("onResult:false");
            }


        }
        private class JsonSerializerStrategy : SimpleJson.PocoJsonSerializerStrategy
        {
            // convert string to int
            public override object DeserializeObject(object value, Type type)
            {
                if (type == typeof(Int32) && value.GetType() == typeof(string))
                {
                    return Int32.Parse(value.ToString());
                }
                return base.DeserializeObject(value, type);
            }
        }


    }

    [Serializable]
    public class AitalkWord
    {
        public int id;
        public string word;
        public UnityEvent action;
    }


    public enum RecognizedStatus
    {
        None,
        Init,//初始化完成
        BuildSuccess,//构建成功
        BuildFail,//构建失败
        BeginOfSpeech,//开始说话
        EndOfSpeech,//结束输入
        Error,//
        Result,//

    }
}