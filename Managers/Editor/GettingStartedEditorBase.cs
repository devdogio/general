using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Net;

namespace Devdog.General.Editors
{
    public abstract class GettingStartedEditorBase : EditorWindow
    {
        protected const int SingleColWidth = 400;
        protected static Texture documentationIcon;
        protected static Texture videoTutorialsIcon;
        protected static Texture reviewIcon;
        protected static Texture forumIcon;
        protected static Texture discordIcon;
        protected static Texture integrationsIcon;
        protected static Texture newsletterIcon;
        protected static Texture devdogLogoIcon;
        protected static Texture newsletterBackground;

        public bool showOnStart = true;
        protected int heightExtra;

        protected string productName;
        protected string documentationUrl;
        protected string productsFetchApiUrl;
        protected string youtubeUrl;
        protected string forumUrl = "http://forum.devdog.io";
        protected string discordUrl = "https://discord.gg/zjNj5zZ";
        protected string mailFormUrl = "https://my.sendinblue.com/users/subscribe/js_id/2mfio/id/2";
        protected string reviewProductUrl;
        protected string version;

        private const string IconRootPath = "Assets/Devdog/General/EditorStyles/";
        private const string DocumentationIconUri = IconRootPath + "Documentation.png";
        private const string VideoTutorialsIconUri = IconRootPath + "Youtube.png";
        private const string ReviewIconUri = IconRootPath + "Star.png";
        private const string ForumIconUri = IconRootPath + "Forum.png";
        private const string DiscordIconUri = IconRootPath + "Discord.png";
        private const string IntegrationIconUri = IconRootPath + "Integration.png";
        private const string NewsletterIconUri = IconRootPath + "MailNotSignedUp.png";
        private const string DevdogLogoIconUri = IconRootPath + "DevdogLogo.png";
        private const string NewsletterBackgroundUri = IconRootPath + "MailBG.png";

        private static string _email;

        public static GettingStartedEditorBase window { get; protected set; }

        public string editorPrefsKey
        {
            get { return "SHOW_" + productName + "_GETTING_STARTED_WINDOW" + version; }
        }

        public void GetImages()
        {
            documentationIcon = AssetDatabase.LoadAssetAtPath<Texture>(DocumentationIconUri);
            videoTutorialsIcon = AssetDatabase.LoadAssetAtPath<Texture>(VideoTutorialsIconUri);
            reviewIcon = AssetDatabase.LoadAssetAtPath<Texture>(ReviewIconUri);
            forumIcon = AssetDatabase.LoadAssetAtPath<Texture>(ForumIconUri);
            discordIcon = AssetDatabase.LoadAssetAtPath<Texture>(DiscordIconUri);
            integrationsIcon = AssetDatabase.LoadAssetAtPath<Texture>(IntegrationIconUri);
            devdogLogoIcon = AssetDatabase.LoadAssetAtPath<Texture>(DevdogLogoIconUri);
            newsletterBackground = AssetDatabase.LoadAssetAtPath<Texture>(NewsletterBackgroundUri);

            newsletterIcon = AssetDatabase.LoadAssetAtPath<Texture>(NewsletterIconUri);
        }

        public abstract void ShowWindow();

        protected void OnGUI()
        {
            heightExtra = 0;
            if (window == null)
            {
                ShowWindow();
                return;
            }
            
            GUILayout.BeginHorizontal("Toolbar", GUILayout.Width(window.position.size.x));
            GUILayout.Label(productName + " Version: " + version);
            GUILayout.EndVertical();

            GUILayout.BeginArea(new Rect(0, 0, SingleColWidth, window.position.height));
            DrawGettingStarted();
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(SingleColWidth + (SingleColWidth / 50), 0, SingleColWidth + 10, window.position.height));
            DrawMailSignupForm();
            GUILayout.EndArea();

            GUI.DrawTexture(new Rect(position.width - 128f, position.height - 128f, 128f, 128f), devdogLogoIcon);
        }

        protected void DrawMailSignupForm()
        {
            GUI.DrawTexture(new Rect(SingleColWidth / 2 - 32, 30, 64, 64), newsletterIcon);
            GUI.DrawTexture(new Rect(10,-20,412,360), newsletterBackground);

            GUILayout.Space(100);
            

            EditorGUILayout.LabelField("Join 16k+ game devs on the Devdog newsletter.", UnityEditor.EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Never miss a giveaway, update or product launch.");

            GUILayout.Space(20);

            GUI.color = Color.green;
            if (GUILayout.Button("Subscribe to mailing list", "LargeButton", GUILayout.Width(SingleColWidth)))
            {
                Application.OpenURL(mailFormUrl);
            }
            GUI.color = Color.white;
        }

        protected virtual void DrawGettingStarted()
        {
            var toggle = GUI.Toggle(new Rect(10, window.position.height - 20, SingleColWidth - 10, 20), showOnStart, "Show " + productName + " getting started on start.");
            if (toggle != showOnStart)
            {
                showOnStart = toggle;
                EditorPrefs.SetBool(editorPrefsKey, toggle);
            }
        }

        protected void DrawBox(int index, int extraHeight, string title, string desc, Texture texture, Action action)
        {
            heightExtra += extraHeight;

            const int spacing = 10;
            const int offset = 30;
            int offsetY = offset + (spacing * index) + (64 * index);

            GUI.BeginGroup(new Rect(10, offsetY, SingleColWidth - 20, 64 + heightExtra), Devdog.General.Editors.EditorStyles.boxStyle);

            var rect = new Rect(0, 0, SingleColWidth - 20, 64 + heightExtra);
            if (GUI.Button(rect, GUIContent.none, GUIStyle.none))
            {
                action();
            }

            EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);

            var iconRect = new Rect(15, 7, 50, 50);
            GUI.DrawTexture(iconRect, texture);

            rect.x = 74;
            rect.y += 5;
            rect.width = SingleColWidth - 90;

            GUI.Label(rect, title, UnityEditor.EditorStyles.boldLabel);

            rect.y += 20;
            rect.height = 44;
            GUI.Label(rect, desc, UnityEditor.EditorStyles.wordWrappedLabel);

            GUI.EndGroup();


        }

        private bool Button(GUIContent content)
        {
            return GUILayout.Button(content, GUILayout.Width(128), GUILayout.Height(128));
        }
    }
}