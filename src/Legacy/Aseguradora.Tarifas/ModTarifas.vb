Option Strict On
Option Explicit On

' ---------------------------------------------------------------------------
'  Tabla de tarifas de autos.
'  Escrita en 2009 para el cotizador de ventanilla. "NO TOCAR sin hablar con
'  Actuaría" (comentario original). Código y datos 100% ficticios para la demo.
' ---------------------------------------------------------------------------
Public Module ModTarifas

    ''' <summary>Tasa base anual sobre la suma asegurada, por tipo de vehículo.</summary>
    Public Function ObtenerTasaBase(ByVal tipoVehiculo As String) As Decimal
        Select Case tipoVehiculo
            Case "SEDAN"
                Return 0.035D
            Case "PICKUP"
                Return 0.042D
            Case "MOTO"
                Return 0.06D
            Case Else
                ' MICROBUS y cualquier tipo nuevo caen aquí: "tasa general".
                Return 0.05D
        End Select
    End Function

    ''' <summary>Factor de ajuste por zona de circulación.</summary>
    Public Function FactorZona(ByVal zona As String) As Decimal
        Select Case zona
            Case "URBANA"
                Return 1.1D
            Case "RURAL"
                Return 0.95D
            Case Else
                Return 1D
        End Select
    End Function

End Module
