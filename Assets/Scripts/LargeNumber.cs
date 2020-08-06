using System;

public class LargeNumber {
	static string formatString = "N2";
	static double maxFractional = 0.99;

	static string[] latin = {"mi", "bi", "tri", "quadri", "quinti", "sexti", "septi", "octi", "noni"};
	static string[] ones = {"un", "duo", "tre", "quattuor", "quinqua", "se", "septe", "octo", "nove"};
	static string[] tens = {"deci", "viginti", "triginta", "quadraginta", "quinquaginta", "sexaginta", "septuaginta", "octoginta", "nonaginta"};
	static string[] hundreds = {"centi", "ducenti", "trecenti", "quadringenti", "quingenti", "sescenti", "septingenti", "octingenti", "nongenti"};

	static public string ToString (double rawNumber) {
		if (rawNumber < 1000000.0) {
			return rawNumber.ToString (formatString);
		}
		ScientificNotation scientificNotation = ScientificNotation.FromDouble (rawNumber);
		ushort adjustedExponent = (ushort)((scientificNotation.exponent / 3) - 1);
		string prefix = "";
		if (adjustedExponent < 10) {
			prefix = latin[adjustedExponent - 1];
		}
		else {
			ushort hundredsPlace = (ushort)(adjustedExponent / 100);
			ushort tensPlace = (ushort)((adjustedExponent / 10) % 10);
			ushort onesPlace = (ushort)(adjustedExponent % 10);
			string onesString = (onesPlace > 0) ? ones[onesPlace - 1] : "";
			string modifier = "";
			if ((onesPlace == 7) || (onesPlace == 9)) {
				if (tensPlace > 0) {
					if ((tensPlace == 2) || (tensPlace == 8)) {
						modifier = "m";
					}
					else if (tensPlace != 9) {
						modifier = "n";
					}
				}
				else if (hundredsPlace > 0) {
					if (hundredsPlace == 8) {
						modifier = "m";
					}
					else if (hundredsPlace != 9) {
						modifier = "n";
					}
				}
			}
			if ((onesPlace == 3) || (onesPlace == 6)) {
				if (tensPlace > 0) {
					if ((tensPlace == 2) || (tensPlace == 3) || (tensPlace == 4) || (tensPlace == 5) || (tensPlace == 8)) {
						modifier = ((onesPlace == 6) && (tensPlace == 8)) ? "x" : "s";
					}
				}
				else if (hundredsPlace > 0) {
					if ((hundredsPlace == 1) || (hundredsPlace == 3) || (hundredsPlace == 4) || (hundredsPlace == 5) || (hundredsPlace == 8)) {
						modifier = ((onesPlace == 6) && ((tensPlace == 1) || (tensPlace == 8))) ? "x" : "s";
					}
				}
			}
			string tensString = (tensPlace > 0) ? tens[tensPlace - 1] : "";
			string hundredsString = (hundredsPlace > 0) ? hundreds[hundredsPlace - 1] : "";
			prefix = string.Format ("{0}{1}{2}{3}", onesString, modifier, tensString, hundredsString);
		}
		double adjustedSignificand = scientificNotation.significand * Math.Pow (10, scientificNotation.exponent % 3);
		double integralPart = Math.Truncate (adjustedSignificand);
		return string.Format("{0} {1}llion", (((adjustedSignificand - integralPart) > maxFractional) ? integralPart + maxFractional : adjustedSignificand).ToString (formatString), prefix.TrimEnd ('a'));
	}
}

public struct ScientificNotation {
	public ushort exponent;
	public double significand;

	static public ScientificNotation FromDouble (double rawNumber) {
		ushort exponent = (ushort)Math.Log10 (rawNumber);
		return new ScientificNotation {
			exponent = exponent,
			significand = rawNumber * Math.Pow (0.1, exponent)
		};
	}
}
