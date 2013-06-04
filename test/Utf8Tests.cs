// Copyright (c) ThunderMain Ltd. All rights reserved. See LICENSE in the project root for license information.
using System.IO;
using System.Text;
using System.Xml;
using Xunit;

namespace ThunderMain.GedcomReader.Test
{
    public class Utf8Tests
    {
        private const string Transmission = "\ufeff0 Items\n" +
            "1 Item @attr1@ Test with an entity: \n" +
            "1 @attr2@ Item test with a child element \n" +
            "2 more\n" +
            "1 Item test with a CDATA section  def\n" +
            "1 Item Test with an char entity: &\n" +
            "1 Item 1234567890ABCD\n" +
            "1 Item Test with Unicode char: ☺";

        [Fact]
        public void TransmissionParsesSuccessfully()
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(Transmission)))
            {
                var dom = new XmlDocument();
                dom.Load(new XmlReaderDecorator(new GedcomReader(stream)));

                const string expectedOutput = "<GED><Items><Item REF=\"attr1\">Test with an entity: </Item><Item ID=\"attr2\">test with a child element <more></more></Item><Item>test with a CDATA section  def</Item><Item>Test with an char entity: &amp;</Item><Item>1234567890ABCD</Item><Item>Test with Unicode char: ☺</Item></Items></GED>";
                Assert.Equal(expectedOutput, dom.DocumentElement.OuterXml);
            }
        }
    }
}
