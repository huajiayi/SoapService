using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using WebServiceDemo.Models;
using WebServiceDemo.Helper;
using MHCC.Common.DataAccess;
using MHCC.Common.DataAccess.Models;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.IO;
using System.Threading;
using System.Text;

namespace WebServiceDemo
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {
        private const string PathPacked = "Custom";
        private const string Salt = "jtNopowXQ4U=";
        private const string Secret = "bWhjY19yZF9vbmx5";
        private const string PathRecordings = "Recordings";
        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod(Description = "获取采集定义路径列表方法")]
        public string[] GetMeasurementPaths()
        {
            List<MeasurementPath> measurementPathsList = SQLHelper.Query<MeasurementPath>("select [Machine].[Name] as [MachineName], [Sensor].[Name] as [SensorName], [Measurement].[Name] as [MeasurementName], [Measurement].[MeasurementID] from [Machine], [Sensor], [Measurement] where [Measurement].[SensorID] = [Sensor].[SensorID] and [Sensor].[MachineID] = [Machine].[MachineID] order by [Machine].[Name], [Sensor].[Name], [Measurement].[Name]").ToList<MeasurementPath>();

            string[] measurementPaths = new string[measurementPathsList.Count];
            for (int i = 0; i < measurementPaths.Length; i++)
            {
                measurementPaths[i] = measurementPathsList[i].MachineName + "\\" + measurementPathsList[i].SensorName + "\\" + measurementPathsList[i].MeasurementName;
            }

            return measurementPaths;
        }

        [WebMethod(Description = "获取采集记录方法")]
        public string[] GetRecordings(string @measurementPath, string startTime, string finishTime)
        {
            //从数据库获取采集定义路径
            List<MeasurementPath> measurementPathsList = SQLHelper.Query<MeasurementPath>("select [Machine].[Name] as [MachineName], [Sensor].[Name] as [SensorName], [Measurement].[Name] as [MeasurementName], [Measurement].[MeasurementID] from [Machine], [Sensor], [Measurement] where [Measurement].[SensorID] = [Sensor].[SensorID] and [Sensor].[MachineID] = [Machine].[MachineID] order by [Machine].[Name], [Sensor].[Name], [Measurement].[Name]").ToList<MeasurementPath>();

            string[] measurementPaths = new string[measurementPathsList.Count];
            for (int i = 0; i < measurementPaths.Length; i++)
            {
                measurementPaths[i] = measurementPathsList[i].MachineName + "\\" + measurementPathsList[i].SensorName + "\\" + measurementPathsList[i].MeasurementName;
            }

            //取出与采集定义路径对应的采集定义ID
            string measurementID = null;
            for (int i = 0; i < measurementPaths.Length; i++)
            {
                if (@measurementPath.Equals(measurementPaths[i]))
                {
                    measurementID = measurementPathsList[i].MeasurementID.ToString();
                }
            }

            //取出记录
            List<RecordingDapper> RecordingDapperList = SQLHelper.Query<RecordingDapper>(string.Format("select [Timestamp], [MeasurementType], [AlarmStatus], [BiasVoltage], [Delta], [Offset], [Overall], [RPM], [XUnit], [Count] as [YCount],[YUnit] from [Recording] where [MeasurementID] = N'{0}' and [Timestamp] between N'{1}' and N'{2}'", measurementID, startTime, finishTime)).ToList<RecordingDapper>();
            //转换MeasurementType
            Dictionary<string, int> MeasurementTypeDictionary = new Dictionary<string, int>();
            MeasurementTypeDictionary.Add("TimeSeries", 1);
            MeasurementTypeDictionary.Add("AutoSpectrum", 2);
            MeasurementTypeDictionary.Add("EnvelopeAutoSpectrum", 3);
            MeasurementTypeDictionary.Add("TrackedAutoSpectrum", 4);
            MeasurementTypeDictionary.Add("TrackedEnvelopeAutoSpectrum", 5);
            MeasurementTypeDictionary.Add("RmsOverall", 6);
            MeasurementTypeDictionary.Add("RpmProfile", 7);
            MeasurementTypeDictionary.Add("Peak", 8);
            MeasurementTypeDictionary.Add("Skewness", 9);
            MeasurementTypeDictionary.Add("Kurtosis", 10);
            MeasurementTypeDictionary.Add("CrestFactor", 11);
            MeasurementTypeDictionary.Add("RmsHarmonic", 12);
            MeasurementTypeDictionary.Add("RpmFFT", 13);
            MeasurementTypeDictionary.Add("AverageValue", 14);
            MeasurementTypeDictionary.Add("Singular", 15);
            MeasurementTypeDictionary.Add("BulkValue", 16);
            MeasurementTypeDictionary.Add("OPC", 17);
            MeasurementTypeDictionary.Add("PeakToPeak", 18);

            List<WebServiceDemo.Models.Recording> recordings = new List<WebServiceDemo.Models.Recording>();
            for (int i = 0; i < RecordingDapperList.Count; i++)
            {
                WebServiceDemo.Models.Recording recording = new WebServiceDemo.Models.Recording();
                recording.AlarmStatus = RecordingDapperList[i].AlarmStatus;
                recording.BiasVoltage = RecordingDapperList[i].BiasVoltage;
                recording.Delta = RecordingDapperList[i].Delta;
                recording.MeasurementPath = @measurementPath;
                recording.MeasurementType = MeasurementTypeDictionary[RecordingDapperList[i].MeasurementType];
                recording.Offset = RecordingDapperList[i].Offset;
                recording.Overall = RecordingDapperList[i].Overall;
                recording.RPM = RecordingDapperList[i].RPM;
                recording.Timestamp = RecordingDapperList[i].Timestamp;
                recording.XUnit = RecordingDapperList[i].XUnit;
                recording.YCount = RecordingDapperList[i].YCount;
                recording.YUnit = RecordingDapperList[i].YUnit;
                //获取YValues
                List<RecordingDapper> TypeList = SQLHelper.Query<RecordingDapper>(string.Format("select [Type] from [MDataPort].[dbo].[Recording] where [MeasurementID] = N'{0}' and [Timestamp] = N'{1}'", measurementID, recording.Timestamp.ToString("o"))).ToList<RecordingDapper>();
                float[] fType = GetYValues(DateTime.Parse(recording.Timestamp.ToString("o")), Guid.Parse(measurementID), TypeList[0].Type).ToArray();
                double[] dType = new double[fType.Length];
                for (int f = 0; f < fType.Length; f++)
                {
                    dType[f] = double.Parse(fType[f].ToString());
                }
                recording.YValues = dType;

                //获取报警记录
                List<WebServiceDemo.Models.Alarm> AlarmList = SQLHelper.Query<WebServiceDemo.Models.Alarm>(string.Format("select [AlarmDefinition].[Name], [AlarmDefinition].[Type] as [AlarmType], [AlarmDefinition].[FrequenceStart], [AlarmDefinition].[FrequenceEnd], [AlarmRecord].[CalculatedValue], [Recording].[AlarmStatus] from [Alarm], [AlarmDefinition], [AlarmRecord], [AlarmThreshold], [Recording] where [Recording].[MeasurementID] = N'{0}' and [Recording].[Timestamp] = N'{1}' and [Recording].[MeasurementID] = [AlarmRecord].[MeasurementID] and [Recording].[Timestamp] = [AlarmRecord].[Timestamp] and [AlarmRecord].[ThresholdID] = [AlarmThreshold].[AlarmThresholdID] and [AlarmRecord].[AlarmID] = [Alarm].[AlarmID] and [AlarmThreshold].[AlarmDefinitionID] = [AlarmDefinition].[AlarmDefinitionID]", measurementID, recording.Timestamp.ToString("o"))).ToList<WebServiceDemo.Models.Alarm>();
                List<WebServiceDemo.Models.AlarmThreshold> AlarmThresholdList = SQLHelper.Query<WebServiceDemo.Models.AlarmThreshold>(string.Format("select [AlarmIfLessThan], [Level], [Value] from [AlarmThreshold] where [AlarmDefinitionID] = (select [AlarmDefinitionID] from [AlarmDefinition] where [MeasurementID] = N'{0}')", measurementID)).ToList<WebServiceDemo.Models.AlarmThreshold>();
                if (AlarmList != null)
                {
                    for (int j = 0; j < AlarmList.Count; j++)
                    {
                        //转换AlarmType
                        switch (AlarmList[j].AlarmType)
                        {
                            case 0:
                                AlarmList[j].AlarmType = 1;
                                break;
                            case 1:
                                AlarmList[j].AlarmType = 2;
                                break;
                            case 5:
                                AlarmList[j].AlarmType = 3;
                                break;
                            case 2:
                                AlarmList[j].AlarmType = 4;
                                break;
                        }

                        //转换阈值信息
                        for (int k = 0; k < AlarmThresholdList.Count; k++)
                        {
                            if (AlarmThresholdList[k].AlarmIfLessThan == 0 && AlarmThresholdList[k].Level == 1)
                            {
                                AlarmList[j].ThresholdWarning = AlarmThresholdList[k].Value;
                            }
                            if (AlarmThresholdList[k].AlarmIfLessThan == 0 && AlarmThresholdList[k].Level == 2)
                            {
                                AlarmList[j].ThresholdAlert = AlarmThresholdList[k].Value;
                            }
                            if (AlarmThresholdList[k].AlarmIfLessThan == 0 && AlarmThresholdList[k].Level == 3)
                            {
                                AlarmList[j].ThresholdDanger = AlarmThresholdList[k].Value;
                            }
                            if (AlarmThresholdList[k].AlarmIfLessThan == 1 && AlarmThresholdList[k].Level == 1)
                            {
                                AlarmList[j].ThresholdMinusWarning = AlarmThresholdList[k].Value;
                            }
                            if (AlarmThresholdList[k].AlarmIfLessThan == 1 && AlarmThresholdList[k].Level == 2)
                            {
                                AlarmList[j].ThresholdMinusAlert = AlarmThresholdList[k].Value;
                            }
                            if (AlarmThresholdList[k].AlarmIfLessThan == 1 && AlarmThresholdList[k].Level == 3)
                            {
                                AlarmList[j].ThresholdMinusDanger = AlarmThresholdList[k].Value;
                            }
                        }
                    } 
                }

                //将Alarm集合转换成Alarm数组
                WebServiceDemo.Models.Alarm[] alarmArr = new WebServiceDemo.Models.Alarm[AlarmList.Count];
                for (int l = 0; l < alarmArr.Length; l++)
                {
                    WebServiceDemo.Models.Alarm alarm = new WebServiceDemo.Models.Alarm();
                    alarm.CalculatedValue = AlarmList[l].CalculatedValue;
                    alarm.FrequenceStart = AlarmList[l].FrequenceStart;
                    alarm.FrequenceEnd = AlarmList[l].FrequenceEnd;
                    alarm.Name = AlarmList[l].Name;
                    alarm.ThresholdWarning = AlarmList[l].ThresholdWarning;
                    alarm.ThresholdAlert = AlarmList[l].ThresholdAlert;
                    alarm.ThresholdDanger = AlarmList[l].ThresholdDanger;
                    alarm.ThresholdMinusWarning = AlarmList[l].ThresholdMinusWarning;
                    alarm.ThresholdMinusAlert = AlarmList[l].ThresholdMinusAlert;
                    alarm.ThresholdMinusDanger = AlarmList[l].ThresholdMinusDanger;
                    alarm.AlarmStatus = AlarmList[l].AlarmStatus;
                    alarm.AlarmType = AlarmList[l].AlarmType;
                    alarmArr[l] = alarm;
                }

                recording.Alarms = alarmArr;

                recordings.Add(recording);
            }

            //将结果转换成Json格式
            string[] recordingJsons = new string[recordings.Count];
            for (int i = 0; i < recordingJsons.Length; i++)
            {
                string recordingJson = JsonConvert.SerializeObject(recordings[i]);
                recordingJsons[i] = recordingJson;
            }

            return recordingJsons;
        }

        public IEnumerable<float> GetYValues(DateTime timestamp, Guid measurementID, int type)
        {
            var binaryBase = string.Empty;
            const string sql =
                @"
SELECT [Value]
  FROM [Option] o
WHERE o.[OptionID] = 'OPTION_BINARY_BASE'
";
            binaryBase = Repository.Query<string>(sql).FirstOrDefault();

            const string select = @"
SELECT TOP 1 m.[SiteID], r.[Hash]
FROM [Recording] r
	LEFT JOIN [Measurement] meas ON meas.[MeasurementID] = r.[MeasurementID]
	LEFT JOIN [Sensor] s ON s.[SensorID] = meas.[SensorID]
	LEFT JOIN [Machine] m ON m.[MachineID] = s.[MachineID]
WHERE r.[MeasurementID] = @MeasurementID AND [Timestamp] = @Timestamp AND r.[Type] = @Type";

            var temp = Repository.Query<dynamic>(select, new Dictionary<string, object>
            {
                {"@MeasurementID", measurementID},
                {"@Timestamp", timestamp},
                {"@Type", type}
            }).First();

            Guid siteID = temp.SiteID;
            string hash = temp.Hash;

            if (string.IsNullOrWhiteSpace(hash))
            {
                return null;
            }

            // If the recording is packed
            if (hash.IndexOf("_", StringComparison.OrdinalIgnoreCase) > -1)
            {
                var recordings = UnPackage(siteID, timestamp, hash, binaryBase);
                PutRecordingsIntoCache(DateTimeOffset.Now.AddHours(24), recordings);

                return recordings.First(o => o.Timestamp == timestamp &&
                                             o.MeasurementID == measurementID &&
                                             o.Type == type).YValues;
            }

            var values = GetYValues(siteID, timestamp, hash, binaryBase);


            return values;
        }
        private void Decrypt(string path, string secret, string salt, Action<Stream> action)
        {
            // generate the key from the shared secret and the salt
            var key = new Rfc2898DeriveBytes(secret, Convert.FromBase64String(salt));

            // Create the stream used for encryption
            using (var algorithm = new RijndaelManaged())
            {
                Stream stream = null;
                try
                {
                    try
                    {
                        stream = File.OpenRead(path);
                    }
                    catch (IOException)
                    {
                        Thread.Sleep(5000);
                        stream = new FileStream(path, FileMode.Open);
                    }

                    algorithm.Key = key.GetBytes(algorithm.KeySize / 8);

                    // Get the initialization vector from the encrypted stream
                    algorithm.IV = ReadByteArray(stream);

                    // create encryptor to perform the stream transform.
                    using (var decryptor = algorithm.CreateDecryptor(algorithm.Key, algorithm.IV))
                    using (var cs = new CryptoStream(stream, decryptor, CryptoStreamMode.Read))
                    {
                        action(cs);
                    }
                }
                finally
                {
                    if (stream != null)
                    {
                        stream.Dispose();
                    }
                }
            }
        }

        private byte[] ReadByteArray(Stream s)
        {
            var rawLength = new byte[sizeof(int)];
            if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
            {
                throw new SystemException("Stream did not contain properly formatted byte array");
            }

            var buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
            if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                throw new SystemException("Did not read byte array properly");
            }

            return buffer;
        }

        private void Decrypt(string path, string secret, string salt, Action<ExtendBinaryReader> action)
        {
            byte[] bytes = null;
            Decrypt(path,
                secret,
                salt,
                (Stream cs) =>
                {
                    using (var ms = new MemoryStream())
                    {
                        var buffer = new byte[1024];
                        var read = cs.Read(buffer, 0, buffer.Length);
                        while (read > 0)
                        {
                            ms.Write(buffer, 0, read);
                            read = cs.Read(buffer, 0, buffer.Length);
                        }

                        bytes = ms.ToArray();
                    }
                });

            using (var ms = new MemoryStream(bytes))
            using (var reader = new ExtendBinaryReader(ms))
            {
                action(reader);
            }
        }

        public float[] GetYValues(Guid siteID, DateTime timestamp, string hash, string binaryBase)
        {
            var path = Path.Combine(binaryBase,
                PathRecordings,
                siteID.ToString(),
                timestamp.Year.ToString(),
                timestamp.Month.ToString(),
                timestamp.Day.ToString(),
                hash);

            float[] yvalues = null;

            if (File.Exists(path))
            {
                if (new FileInfo(path).Length == 0)
                {
                    return null;
                }

                try
                {
                    Decrypt(path,
                        Secret,
                        Salt,
                        reader =>
                        {
                            // Get length of the metadata part
                            var length = reader.Read7BitEncodedInt();

                            var bytes = reader.ReadBytes(length);
                            var txt = Encoding.UTF8.GetString(bytes);
                            var metadata = JsonConvert.DeserializeObject<RecordingMetadata>(txt);

                            yvalues = new float[metadata.Count];

                            var count = (int)(reader.BaseStream.Length - reader.BaseStream.Position);

                            if (count == metadata.Count * 8)
                            {
                                for (var i = 0; i < metadata.Count; i++)
                                {
                                    yvalues[i] = (float)reader.ReadDouble();
                                }
                            }
                            else if (count == metadata.Count * 4)
                            {
                                for (var i = 0; i < metadata.Count; i++)
                                {
                                    yvalues[i] = reader.ReadSingle();
                                }
                            }
                        });
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            else
            {
            }

            //Log.Info("Take {0}ms to read yvalues from file", sw.ElapsedMilliseconds);

            return yvalues;
        }

        private void PutRecordingsIntoCache(DateTimeOffset expiration,
         IReadOnlyList<MHCC.Common.DataAccess.Models.Recording> recordings)
        {
            if (recordings == null || recordings.Count == 0)
            {
                return;
            }

            foreach (var recording in recordings)
            {
                if (recording.YValues == null)
                {
                    return;
                }

                var key = GetRecordingKey(recording.Timestamp, recording.MeasurementID, recording.Type);
            }
        }

        private string GetRecordingKey(DateTime timestamp, Guid measurementID, int type)
        {
            return timestamp.Ticks + measurementID.ToString("N") + type;
        }

        public IReadOnlyList<MHCC.Common.DataAccess.Models.Recording> UnPackage(Guid siteID, DateTime date, string hash,
           string binaryBase)
        {
            if (string.IsNullOrWhiteSpace(hash) ||
                hash.IndexOf("_", StringComparison.OrdinalIgnoreCase) < 0)
            {
                return null;
            }

            var path = Path.Combine(binaryBase,
                PathPacked,
                siteID.ToString(),
                date.Year.ToString(),
                date.Month.ToString(),
                date.Day.ToString(),
                hash);

            IReadOnlyList<MHCC.Common.DataAccess.Models.Recording> recordings = null;

            Decrypt(path, Secret, Salt, reader =>
            {
                var version = reader.ReadByte();
                switch (version)
                {
                    case 1:
                    default:
                        recordings = ParseVersion1(hash, reader);
                        break;
                }
            });

            return recordings;
        }

        private IReadOnlyList<MHCC.Common.DataAccess.Models.Recording> ParseVersion1(string hash,
          ExtendBinaryReader reader)
        {
            var headerLength = reader.ReadInt32();
            var dataLength = reader.ReadInt32();

            var bytes = reader.ReadBytes(headerLength);

            var headerString = Encoding.UTF8.GetString(bytes);

            var header = JsonConvert.DeserializeObject<Header>(headerString);

            var result = new List<MHCC.Common.DataAccess.Models.Recording>();

            foreach (var meas in header.Measurements)
            {
                foreach (var recording in meas.Recordings)
                {
                    var values = new float[meas.Count];

                    // 1 for version byte
                    // 4 for header length bytes
                    // 4 for data length bytes
                    reader.BaseStream.Position = 1 + 4 + 4 + headerLength + recording.StartIndex;

                    for (var i = 0; i < meas.Count; i++)
                    {
                        values[i] = reader.ReadSingle();
                    }

                    result.Add(new MHCC.Common.DataAccess.Models.Recording
                    {
                        Timestamp = recording.Timestamp,
                        MeasurementID = meas.MeasurementID,
                        Type = recording.Type,
                        AlarmStatus = recording.Alarmstatus,
                        BiasVoltage = recording.BiasVoltage,
                        Hash = hash,
                        MeasurementType = meas.MeasurementType,
                        Overall = recording.Overall,
                        RPM = recording.RPM,
                        Delta = recording.Delta,
                        Offset = recording.Offset,
                        XUnit = meas.XUnit,
                        Count = meas.Count,
                        YUnit = meas.YUnit,
                        //AlarmCalculated = true,
                        YValues = values
                    });
                }
            }

            return result;
        }

    }
}
