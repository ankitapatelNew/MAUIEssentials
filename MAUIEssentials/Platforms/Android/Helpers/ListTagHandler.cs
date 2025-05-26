using Android.Text;
using Org.Xml.Sax;

namespace MAUIEssentials.Platforms.Android.Helpers
{
    public class ListTagHandler : Java.Lang.Object, Html.ITagHandler
    {
        public const string TagUl = "ULC";
        public const string TagOl = "OLC";
        public const string TagLi = "LIC";

        private ListBuilder _listBuilder;
        public ListTagHandler(int listIndent)
        {
            _listBuilder = new ListBuilder(listIndent);
        }

        public void HandleTag(bool isOpening, string tag, IEditable output, IXMLReader xmlReader)
        {
            tag = tag.ToUpperInvariant();
            var isItem = tag == TagLi;

            if (isItem)
            {
                _listBuilder.AddListItem(isOpening, output);
            }
            else
            {
                if (isOpening)
                {
                    var isOrdered = tag == TagOl;
                    _listBuilder = _listBuilder.StartList(isOrdered, output);
                }
                else
                {
                    _listBuilder = _listBuilder.CloseList(output);
                }
            }
        }
    }
}
