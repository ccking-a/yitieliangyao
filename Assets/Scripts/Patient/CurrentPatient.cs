using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CurrentPatient
{
    public static PatientData data;
    public static bool isDiagnosis;
    public static bool isTreatment;
    public static void SetCurrentPatient(PatientData data)
    {
        CurrentPatient.data = data;
        CurrentPatient.isDiagnosis = false;
        CurrentPatient.isTreatment = false;
    }
}
