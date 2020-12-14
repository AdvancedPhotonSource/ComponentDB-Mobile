using System;
namespace Component.DB.Services
{
    public class QrMessage
    {

        public static String MESSAGE_SCANNED_TOPIC = "Barcode-Scan";

        public static String QRCODE_CODETYPE = "QRCODE";
        public static String QRCODE_CODETYPE2 = "QR_CODE";

        public static String CODE128_CODETYPE = "CODE128"; 

        public static String QRCODE_START_URL = "qrId=";

        public string CodeType { get; }
        public string CodeContents { get; }

        public QrMessage(String CodeType, String CodeContents)
        {
            this.CodeType = CodeType;
            this.CodeContents = CodeContents;
        }

        public int ParseQrCode()
        {
            String numString = null;
            if (CodeType.Equals(QRCODE_CODETYPE) || CodeType.Equals(QRCODE_CODETYPE2))
            {
                if (CodeContents.Contains(QRCODE_START_URL))
                {
                    int qrIdStartIdx = CodeContents.IndexOf(QRCODE_START_URL);

                    if (qrIdStartIdx > 0)
                    {
                        qrIdStartIdx += 5;
                        numString = CodeContents.Substring(qrIdStartIdx);
                    }
                }
            }
            else if (CodeType.Equals(CODE128_CODETYPE))
            {
                var LowerCode = CodeContents.ToLower(); 
                if (LowerCode.StartsWith(QRCODE_START_URL.ToLower()))
                {
                    numString = LowerCode.Substring(5); 
                }
            }
            else
            {
                //Barcode -- should just contain numbers.
                numString = this.CodeContents;
            }

            if (numString != null)
            {
                // This code can throw exception.
                return Int32.Parse(numString);
            }
            else
            {
                throw new Exception("Could not parse scanned qr code: " + this.CodeContents);
            }
        }
    }
}
