﻿//*********************************************************//
//    Copyright (c) Microsoft. All rights reserved.
//    
//    Apache 2.0 License
//    
//    You may obtain a copy of the License at
//    http://www.apache.org/licenses/LICENSE-2.0
//    
//    Unless required by applicable law or agreed to in writing, software 
//    distributed under the License is distributed on an "AS IS" BASIS, 
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or 
//    implied. See the License for the specific language governing 
//    permissions and limitations under the License.
//
//*********************************************************//

using System;

namespace Microsoft.NodejsTools.Jade {
    class TextChange : ICloneable {
        /// <summary>
        /// Text snapshot version
        /// </summary>
        public int Version;

        /// <summary>
        /// Changed range in the old snapshot.
        /// </summary>
        public TextRange OldRange;

        /// <summary>
        /// Changed range in the current snapshot.
        /// </summary>
        public TextRange NewRange;

        /// <summary>
        /// Previuos text snapshot
        /// </summary>
        public ITextProvider OldText;

        /// <summary>
        /// Current text snapshot
        /// </summary>
        public ITextProvider NewText;

        public TextChange() {
            Clear();
        }

        public TextChange(int start, int oldLength, int newLength) :
            this() {
            OldRange = new TextRange(start, start + oldLength);
            NewRange = new TextRange(start, start + newLength);
        }

        public TextChange(int start, int oldLength, int newLength, ITextProvider oldText, ITextProvider newText)
            : this() {
            this.Combine(new TextChange(start, oldLength, newLength));

            OldText = oldText;
            NewText = newText;
        }

        public TextChange(TextChange change, ITextProvider oldText, ITextProvider newText)
            : this() {
            this.Combine(change);

            OldText = oldText;
            NewText = newText;
        }

        public virtual void Clear() {
            OldRange = TextRange.EmptyRange;
            NewRange = TextRange.EmptyRange;

            OldText = null;
            NewText = null;
        }

        /// <summary>
        /// True if no changes are pending.
        /// </summary>
        public virtual bool IsEmpty() {
            return (OldRange.Length == 0 && NewRange.Length == 0);
        }

        public void Combine(TextChange other) {
            if (other.IsEmpty())
                return;

            if (OldRange == TextRange.EmptyRange || NewRange == TextRange.EmptyRange) {
                OldRange = other.OldRange;
                NewRange = other.NewRange;
            } else {
                int oldStart = Math.Min(other.OldRange.Start, this.OldRange.Start);
                int oldEnd = Math.Max(other.OldRange.End, this.OldRange.End);

                int newStart = Math.Min(other.NewRange.Start, this.NewRange.Start);
                int newEnd = Math.Max(other.NewRange.End, this.NewRange.End);

                this.OldRange = new TextRange(oldStart, oldEnd);
                this.NewRange = new TextRange(newStart, newEnd);
            }

            Version = Math.Max(this.Version, other.Version);
        }

        #region ICloneable Members
        public object Clone() {
            TextChange clone = this.MemberwiseClone() as TextChange;

            clone.OldRange = this.OldRange.Clone() as TextRange;
            clone.NewRange = this.NewRange.Clone() as TextRange;

            return clone;
        }
        #endregion
    }
}
