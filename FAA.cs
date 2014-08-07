using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FAA
{
    public class FAA
    {
        private void LoadTable(DataTable table, Stream stream)
        {
            var timespanExpression = new Regex("([0-9]+):([0-9]+):([0-9]+)");
            var parseDictionary = new Dictionary<Type, Delegate>();
            //parseDictionary.Add(typeof(Boolean), new Func<String, Boolean>(fieldValue => Convert.ToBoolean(Int32.Parse(fieldValue))));
            parseDictionary.Add(typeof(DateTime), new Func<String, Object>(fieldValue => { if(fieldValue.Length.Equals(8)) return new DateTime(Int32.Parse(fieldValue.Substring(0, 4)), Int32.Parse(fieldValue.Substring(4, 2)), Int32.Parse(fieldValue.Substring(6, 2))); else return DBNull.Value; }));
            //parseDictionary.Add(typeof(Decimal), new Func<String, Decimal>(fieldValue => Decimal.Parse(fieldValue)));
            parseDictionary.Add(typeof(Int32), new Func<String, Object>(fieldValue => { Int32 test; if (Int32.TryParse(fieldValue, out test)) return test; else return DBNull.Value; }));
            //parseDictionary.Add(typeof(TimeSpan), new Func<String, Object>(fieldValue => { try { var timeSpanPart = fieldValue.Split(':'); return new TimeSpan(Int32.Parse(timeSpanPart[0]), Int32.Parse(timeSpanPart[1]), Int32.Parse(timeSpanPart[2])); } catch { return DBNull.Value; } }));
            var textFieldParser = new Microsoft.VisualBasic.FileIO.TextFieldParser(stream);
            textFieldParser.TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited;
            textFieldParser.SetDelimiters(",");
            textFieldParser.HasFieldsEnclosedInQuotes = false;
            var fieldReferences = textFieldParser.ReadFields().Where(item=>!String.IsNullOrEmpty(item)).ToArray();
            while (!textFieldParser.EndOfData)
            {
                var fields = textFieldParser.ReadFields();
                var newRow = table.NewRow();
                for (var index = 0; index < fieldReferences.Length; index++)
                {
                    var fieldReference = fieldReferences[index];
                    var fieldValue = fields[index];
                    if (table.Columns.Contains(fieldReference))
                    {
                        if (parseDictionary.ContainsKey(table.Columns[fieldReference].DataType))
                            newRow[fieldReference] = parseDictionary[table.Columns[fieldReference].DataType].DynamicInvoke(fieldValue);
                        else
                            newRow[fieldReference] = fieldValue;
                    }
                }
                table.Rows.Add(newRow);
                //Console.WriteLine(String.Format("{0} {1}",table.TableName, table.Rows.Count));
            }
        }
        public FAA(String zipPath)
        {
            AircraftRegistrationSet = new AircraftRegistrationSet();
            AircraftRegistrationSet._AC_CAT.ReadXml("AC-CAT.xml");
            AircraftRegistrationSet._AC_WEIGHT.ReadXml("AC-WEIGHT.xml");
            AircraftRegistrationSet._CERTIFICATION_Experimental.ReadXml("CERTIFICATION-Experimental");
            AircraftRegistrationSet._CERTIFICATION_Restricted.ReadXml("CERTIFICATION-Restricted");
            AircraftRegistrationSet._CERTIFICATION_Standard.ReadXml("CERTIFICATION-Standard");
            AircraftRegistrationSet._BUILD_CERT_IND.ReadXml("BUILD-CERT-IND.xml");
            AircraftRegistrationSet.REGION.ReadXml("REGION.xml");
            AircraftRegistrationSet.TR.ReadXml("TR.xml");
            AircraftRegistrationSet._TYPE_ACFT.ReadXml("TYPE-ACFT.xml");
            AircraftRegistrationSet._TYPE_COLLATERAL.ReadXml("TYPE-COLLATERAL.xml");
            AircraftRegistrationSet._TYPE_ENG.ReadXml("TYPE-ENG.xml");
            AircraftRegistrationSet.TYPE_REGISTRANT.ReadXml("TYPE_REGISTRANT.xml");
            using (var zipStream = File.OpenRead(zipPath))
            {
                using (var zipArchive = new ZipArchive(zipStream))
                {
                    foreach (var entry in zipArchive.Entries)
                    {
                        if (AircraftRegistrationSet.Tables.Contains(entry.Name))
                        {
                            using (var stream = entry.Open())
                            {
                                var targetTable = AircraftRegistrationSet.Tables[entry.Name];
                                if (targetTable != null)
                                    LoadTable(targetTable, stream);
                            }
                        }
                    }
                }
            }
        }
        public AircraftRegistrationSet AircraftRegistrationSet { get; set; }
    }
}
