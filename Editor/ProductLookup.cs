using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using UnityEditor;

namespace Devdog.General.Editors
{
    public class ProductLookup
    {
        public string name;
        public string niceName;
        private const string RatingPrefixName = "Devdog_Product_Rating_";

        public bool hasReview
        {
            get { return rating != null; }
        }

        public int? rating
        {
            get
            {
                if (EditorPrefs.HasKey(RatingPrefixName + name))
                {
                    return EditorPrefs.GetInt(RatingPrefixName + name, 0);
                }

                return null;
            }
            set
            {
                if (value == null)
                {
                    EditorPrefs.DeleteKey(RatingPrefixName + name);
                }
                else
                {
                    EditorPrefs.SetInt(RatingPrefixName + name, value.GetValueOrDefault(0));
                }
            }
        }

        public string email = "";
        public string feedback = "";

        public override string ToString()
        {
            return niceName;
        }
    }
}
