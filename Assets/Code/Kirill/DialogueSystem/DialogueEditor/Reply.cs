using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ghosted.Dialogue {
    [System.Serializable]
    public class Reply : IHasChildren
    {
        public string text = "";

        public string Child { get => child; set => child = value; }
        public string Id { get => id; set => id = value; }

        private string id;

        private string child = "";

    }
}
