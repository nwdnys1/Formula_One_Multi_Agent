using System;

namespace DTO
{
    public class JsonStr
    {
        public static string media_interview_start =
        "{" +
            "\"scene_type\": \"media_interview\", " +
            "\"scene_data\": {" +
                "\"sender\": \"unity\", " +
                "\"receiver\": [\"����\", \"ά˹����\", \"���ܶ���\", \"����\"], " +
                "\"signal\": \"media_interview_start\"" +
            "}" +
        "}";
        public static string media_interview_chat(string content)
        {
            return
                "{" +
                    "\"scene_type\": \"media_interview\", " +
                    "\"scene_data\": {" +
                        "\"sender\": \"Wolff\", " +
                        "\"receiver\": [\"����\"], " +
                        "\"content\": \"" + content + "\", " +
                        "\"signal\": \"chat\"" +
                    "}" +
                "}";    
        }
        public static string media_interview_end = "{\r\n    \"scene_type\": \"media_interview\",\r\n    \"scene_data\": {\r\n      \"sender\": \"unity\",\r\n      \"receiver\": \"journalist\",\r\n      \"signal\": \"media_interview_end\"\r\n    }\r\n}";
        public static string practice_session_start = "{\r\n    \"scene_type\": \"practice_session\",\r\n    \"scene_data\": {\r\n      \"sender\": \"unity\",\r\n      \"receiver\": [\"÷�����Ӳ���ʦ\", \"���ܶ���\"], \r\n      \"signal\": \"practice_session_start\" \r\n    }\r\n}";




    }

    [Serializable]
    public class SceneData
    {
        public string sender;
        public string receiver;
        public string content;
    }

    [Serializable]
    public class RequestData
    {
        public string scene_type;
        public SceneData scene_data;
    }
}