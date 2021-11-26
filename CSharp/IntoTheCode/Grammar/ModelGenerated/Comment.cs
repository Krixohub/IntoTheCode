using System.Collections.Generic;

using IntoTheCode.Buffer;
using IntoTheCode.Basic;
using IntoTheCode.Message;
using System;

namespace IntoTheCode.Grammar
{
    internal partial class Comment
    {
        internal Comment()
        {
            CommentBuffer = new List<CommentElement>();
        }
    }
}