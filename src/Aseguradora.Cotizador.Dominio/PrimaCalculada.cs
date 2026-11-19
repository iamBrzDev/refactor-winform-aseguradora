namespace Aseguradora.Cotizador.Dominio
{
    /// <summary>Resultado del cálculo (objeto de valor).</summary>
    public sealed class PrimaCalculada
    {
        public PrimaCalculada(decimal primaNeta, decimal iva)
        {
            PrimaNeta = primaNeta;
            Iva = iva;
        }

        public decimal PrimaNeta { get; }
        public decimal Iva { get; }
        public decimal Total => PrimaNeta + Iva;
    }
}
