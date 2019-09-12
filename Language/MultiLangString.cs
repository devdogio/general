using System;
using UnityEngine;
using System.Collections;


namespace Devdog.General
{
    [System.Serializable]
    public class MultiLangString
    {
        public string title;
        public string message;

        /// <summary>
        /// Required for fsm's
        /// </summary>
        public MultiLangString()
        { }

        public MultiLangString(string title, string message)
        {
            this.title = title;
            this.message = message;
        }

        public override string ToString()
        {
            return MessageToString();
        }

        public string TitleToString(params object[] vars)
        {
            return ToString(title, vars);
        }

        public string MessageToString(params object[] vars)
        {
            return ToString(message, vars);
        }

        private string ToString(string msg, object[] vars)
        {
            try
            {
                return string.Format(msg, vars);
            }
            catch (Exception)
            {
                DevdogLogger.LogWarning("Invalid string.Format :: " + msg);
            }

            return msg;
        }
    }
}