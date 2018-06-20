<%@ page language="C#" autoeventwireup="true" codebehind="Default.aspx.cs" inherits="Ibushak.Productos.UI.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" lang="es">
<head runat="server">
    <title></title>
    <link rel="stylesheet" href="~/Content/css/master.css" type="text/css" />
    <link href="~/Content/bootstrap.min.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="container paddingTop">
            <div class="col-md-6 col-md-offset-3">
                <img id="imgIbushak" runat="server" src="~/Content/imagenes/IbushakG.png" alt="Ibushak" style="max-height: 100%; max-width: 100%;" />
            </div>
            <div class="col-md-6 col-md-offset-3 text-center">
                <asp:Label id="lblError" runat="server" font-bold="true" forecolor="Red"></asp:Label>
            </div>

            <div class="col-md-6 col-md-offset-3">
                <div class="form-group">
                    <label>Usuario</label>
                    <div>
                        <asp:TextBox CssClass="form-control" id="txtUsuario" runat="server" nulltext="Usuario"></asp:TextBox>
                    </div>
                </div>
                <div class="form-group">
                    <label>Contraseña</label>
                    <div>
                        <asp:TextBox CssClass="form-control" TextMode="Password" id="txtContrasenia" runat="server" nulltext="Contraseña" password="true"></asp:TextBox>
                    </div>
                </div>
                <asp:Button CssClass="btn btn-default" id="btnIngresar" runat="server" autopostback="true" text="Inciar Sesión" width="100%" onclick="btnIngresar_Click"></asp:Button>
            </div>
        </div>
    </form>
</body>
</html>