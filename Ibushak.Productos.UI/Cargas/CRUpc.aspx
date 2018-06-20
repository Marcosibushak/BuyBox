<%@ Page Title="" Language="C#" MasterPageFile="~/Productos.Master" AutoEventWireup="true" CodeBehind="CRUpc.aspx.cs" Inherits="Ibushak.Productos.UI.Cargas.CRUpc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="Scriptmanager1" runat="server" />
    <script src="../Scripts/jquery-1.9.1.min.js" language="javascript" type="text/javascript"></script>
    <script type="text/javascript">

        var files = null;

        function loadFiles() {
            files = new Array();
        }

        function Uploader_OnFileComplete(s, e) {
            var index = files.length;
            files[index] = new Object();
            files[index].isValid = e.isValid;
            files[index].callbackData = e.callbackData;
        }

        function UploadComplete(filename) {
            window.rpaProceso.style.display = "block";
            window.divResultados.style.display = "block";
            var info = "";
            var ok = false;

            if (document.getElementById("ContentPlaceHolder1_labelResUpload").innerHTML.includes("OK")) {
                ok = true;
                info += "Archivo No. : 1<br>Resultado   : " + filename + "<br>OK<br><br>";
            }
            document.getElementById("ASPxLabel3").innerHTML = info;
            console.log(info);
            if (ok) {

                window.divUpload.style.display = "none";
                //Barra.style.display = "block";
                window.divResultados.style.display = "block";
                //cbP_proceso.PerformCallback();
            }
        }

        function OnCallbackComplete() {

            //Barra.style.display = "none";
            //rpaProceso.style.display = "none";
            //alert("all right!");
            divResultados.style.display = "block";
        }

        function OnClickHyperLink() {
            divResultados.style.display = "none";
            rpaProceso.style.display = "none";
            divUpload.style.display = "block";
            document.getElementById("ContentPlaceHolder1_labelResUpload").innerHTML = "";
        }

        $(document).ready(function () {

            $("#ContentPlaceHolder1_labelResUpload").on("change",
                function () {

                    console.log("You change Span tag");

                });
        });
    </script>

    <div class="container paddingTop">
        <div id="divUpload" class="row">
            <div class="col-md-6 col-md-offset-3">
                <div class="form-group">
                    <label>Carga de Productos UPC/ASIN</label>
                    <asp:FileUpload ID="bstUploadControlUpc" runat="server" ShowUploadButton="true" onchange=""
                        accept="application/vnd.ms-excel,application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"></asp:FileUpload>
                    <asp:Label runat="server" ID="labelResUpload"></asp:Label>
                    <asp:Button runat="server" Text="upload" ID="buttonUpload" ForeColor="Blue" BackColor="Transparent"
                        BorderColor="Transparent" OnClick="bstUploadControlUpc_FileUploadComplete" />
                </div>
            </div>
        </div>

        <!-- Resultados del Upload y Validación del Archivo -->
        <div align="center" class="row" id="dock_item">
            <div id="rpaProceso" class="col-md-6 col-md-offset-3" style="display: none;">
                <asp:Table
                    ID="ASxPRP_status_proceso"
                    runat="server"
                    Width="400px" Theme="Default"
                    ShowHeader="true" BorderStyle="Solid">
                    <asp:TableHeaderRow Font-Size="Small" CssClass="dxrpHeader" HorizontalAlign="Center" runat="server">
                        <asp:TableCell runat="server" CssClass="dx-vam">
                            <asp:Label runat="server" Text="Resultado de la descarga del Archivo al Servidor"></asp:Label>
                        </asp:TableCell>
                    </asp:TableHeaderRow>
                    <asp:TableRow CssClass="dx-vam2">
                        <asp:TableCell runat="server" CssClass="dx-vam3">
                            <asp:Label ID="ASPxLabel2" runat="server" Text="Archivo(s) recibido(s)" Font-Bold="true">
                            </asp:Label>
                            <br />
                            <br />
                            <div align="left">
                                <span id="ASPxLabel3"></span>
                            </div>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
                <div>
                    <asp:Label runat="server" ID="lbl_mensaje"></asp:Label>
                </div>
                <div>
                    <button onclick="OnClickHyperLink()">Nuevo Archivo</button>
                </div>
            </div>
        </div>

        <!-- CallBack Panel de resultados de la actualizacion de los Documentos en la Base de Datos -->
        <div id="divResultados" align="center" class="row" style="display: none;">
            <div class="col-md-6 col-md-offset-3">
                <%--<dx:BootstrapCallbackPanel ID="cbp_proceso" 
                    runat="server"
                    ClientInstanceName="cbP_proceso" 
                    Width="500px" OnCallback="cbp_proceso_Callback" ShowLoadingPanel="False">
                    <ContentCollection>
                        <dx:ContentControl runat="server">
                                <dx:ASPxLabel ID="lbl_mensaje" runat="server" Text="" EncodeHtml="false" >
                                </dx:ASPxLabel>
                        </dx:ContentControl>
                    </ContentCollection>
                    <ClientSideEvents EndCallback="function(s, e) { OnCallbackComplete(); }" />
                </dx:BootstrapCallbackPanel>
                <dx:BootstrapButton ID="btnCargarNuevoArchivo" runat="server" AutoPostBack="false" Text="Nuevo Archivo">
                    <ClientSideEvents Click="function(s, e) { OnClickHyperLink() }" />
                </dx:BootstrapButton>--%>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        document.getElementById('dock_item').addEventListener("DOMSubtreeModified", handler, true);
        var isWorking = false;
        function handler() {
            if (isWorking === false) {
                isWorking = true;
                loadDoc();
            }
        }
        function loadDoc() {
            var xhttp;
            xhttp = new XMLHttpRequest();
            xhttp.onreadystatechange = function () {
                if (this.readyState == 4 && this.status == 200) {
                    console.log("se termino el proceso");
                }
            };
            xhttp.open("GET", "CRUpc.aspx/CargarArchivo", true);
            xhttp.send();
        }
    </script>
</asp:Content>
    