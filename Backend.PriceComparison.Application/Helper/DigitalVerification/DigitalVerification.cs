namespace Backend.PriceComparison.Application.Helper.DigitalVerification;

public static class DigitalVerification
{
    public static string CalcularDigitoVerificacion(string numero)
    {
        if (string.IsNullOrEmpty(numero) || !long.TryParse(numero, out _))
        {
            throw new ArgumentException("El número ingresado no es válido.");
        }

        int[] pesos = [3, 7, 13, 17, 19, 23, 29, 37, 41, 43, 47, 53, 59, 67, 71];
        var longitud = numero.Length;
        var suma = 0;

        for (var i = 0; i < longitud; i++)
        {
            var digito = int.Parse(numero[longitud - i - 1].ToString());
            suma += digito * pesos[i % pesos.Length];
        }

        var residuo = suma % 11;
        var digitoVerificacion = residuo > 1 ? 11 - residuo : residuo;

        return digitoVerificacion.ToString();
    }
}
