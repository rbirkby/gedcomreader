// Copyright (c) ThunderMain Ltd. All rights reserved. See LICENSE in the project root for license information.
using System;
using System.IO;
using System.Text;
using System.Xml;
using Xunit;

namespace ThunderMain.GedcomReader.Test
{
    public class TestReader
    {
        private const string Transmission = "0 Items\n" +
            "1 Item @attr1@ Test with an entity: \n" +
            "1 @attr2@ Item test with a child element \n" +
            "2 more\n" +
            "1 Item test with a CDATA section  def\n" +
            "1 Item Test with an char entity: &\n" +
            "1 Item 1234567890ABCD\n";

        [Fact]
        public void BasicReadIsSuccessful()
        {
            using (var stream = new MemoryStream(Encoding.ASCII.GetBytes(Transmission)))
            {
                var dom = new XmlDocument();
                dom.Load(new XmlReaderDecorator(new GedcomReader(stream)));

                const string expectedOutput = "<GED><Items><Item REF=\"attr1\">Test with an entity: </Item><Item ID=\"attr2\">test with a child element <more></more></Item><Item>test with a CDATA section  def</Item><Item>Test with an char entity: &amp;</Item><Item>1234567890ABCD</Item></Items></GED>";
                Assert.Equal(expectedOutput, dom.DocumentElement.OuterXml);
            }
        }

        [Fact]
        public void TransmissionWithNonContiguousLevelShouldSucceed()
        {
            const string transmission = "0 Items\n" +
                                        "2 Item Invalid level\n";

            using (var stream = new MemoryStream(Encoding.ASCII.GetBytes(transmission)))
            {
                var dom = new XmlDocument();
                dom.Load(new XmlReaderDecorator(new GedcomReader(stream)));

                Assert.Equal("<GED><Items><Item>Invalid level</Item></Items></GED>", dom.DocumentElement.OuterXml);
                Console.WriteLine(dom.DocumentElement.OuterXml);
            }
        }

        [Fact]
        public void TransmissionWithInvalidLevelValueShouldFail()
        {
            const string transmission = "0 Items\n" +
                                        "100 Item Invalid level value\n";

            using (var stream = new MemoryStream(Encoding.ASCII.GetBytes(transmission)))
            {
                var dom = new XmlDocument();
                Assert.Throws<GedcomException>(() => dom.Load(new XmlReaderDecorator(new GedcomReader(stream))));
            }
        }

        [Fact]
        public void TransmissionWithInvalidLevelCharShouldFail()
        {
            const string transmission = "0 Items\n" +
                                        "X Item Invalid level char\n";

            using (var stream = new MemoryStream(Encoding.ASCII.GetBytes(transmission)))
            {
                var dom = new XmlDocument();
                Assert.Throws<GedcomException>(() => dom.Load(new XmlReaderDecorator(new GedcomReader(stream))));
            }
        }

        [Fact]
        public void RoundTripIsSuccessful()
        {
            const string transmission = "0 Items\n" +
                                        "1 Item first item\n" +
                                        "2 SubItem @attr1@ subitem with attribute\n";

            using (var inputStream = new MemoryStream(Encoding.ASCII.GetBytes(transmission)))
            using (var outputStream = new MemoryStream())
            using (XmlWriter writer = new XmlWriterDecorator(new GedcomWriter(outputStream, Encoding.ASCII)))
            {
                var dom = new XmlDocument();
                dom.Load(new GedcomReader(inputStream));
                dom.Save(writer);

                outputStream.Seek(0, SeekOrigin.Begin);
                Assert.Equal(transmission.ToCharArray(), new StreamReader(outputStream).ReadToEnd().ToCharArray());
            }
        }

        [Fact(Skip = "Not quite roundtrippable yet - doesn't re-order ID attributes")]
        public void RoundTripTransmissionWithIdAttributeIsSuccessful()
        {
            using (var inputStream = new MemoryStream(Encoding.ASCII.GetBytes(Transmission)))
            using (var outputStream = new MemoryStream())
            using (XmlWriter writer = new XmlWriterDecorator(new GedcomWriter(outputStream, Encoding.ASCII)))
            {
                var dom = new XmlDocument();
                dom.Load(new GedcomReader(inputStream));
                dom.Save(writer);

                outputStream.Seek(0, SeekOrigin.Begin);
                Assert.Equal(Transmission.ToCharArray(), new StreamReader(outputStream).ReadToEnd().ToCharArray());
            }
        }
    }
}