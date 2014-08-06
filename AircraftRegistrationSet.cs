using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAA
{
    public partial class AircraftRegistrationSet
    {
        public partial class _MASTER_txtRow
        {
            public enum AIRWORTHYNESS_CERTIFICATE_CLASS
            {
                Standard = 1,
                Limited = 2,
                Restricted = 3,
                Experimental = 4,
                Provisional = 5,
                Multiple = 6,
                Primary = 7,
                Special_Flight_Permit = 8,
                Light_Sport = 9
            }
            public AIRWORTHYNESS_CERTIFICATE_CLASS AIRWORTHYNESS_CLASSIFICATION_CODE { get { return (AIRWORTHYNESS_CERTIFICATE_CLASS)Convert.ToInt32(CERTIFICATION.Substring(0, 1)); } }
        }
    }
}
