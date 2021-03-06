﻿using Csla;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using Seguridad.Entidades;
using ErickOrlando.Seguridad.Entidades;
using ErickOrlando.Seguridad.Login;
using ErickOrlando.Seguridad.WinForms.HelperWin;
using System;
using System.Configuration;
using System.Security;
using System.Windows.Forms;

namespace ErickOrlando.Seguridad.WinForms
{
    /// <summary>
    /// Formulario de Login © 2012
    /// </summary>
    /// <remarks>Erick Orlando © 2012</remarks>
    public partial class LoginForm : DevExpress.XtraEditors.XtraForm
    {

        #region Declaracion de Variables
        public static bool FormAlone = true;
        #endregion

        #region Constructor
        /// <summary>
        /// Inicializa una nueva instancia de LoginForm
        /// </summary>
        public LoginForm()
        {
            InitializeComponent();

            FormClosing += (s, e) =>
                {
                    if (e.CloseReason == CloseReason.UserClosing && FormAlone)
                        Application.Exit();
                };

            Load += (s, e) =>
                {
                    txtUser.Text = Environment.UserName;
                    try
                    {
                        DominiosBindingSource.DataSource = DominioInfoList.GetDominioInfoList();
                        DominiosBindingSource.ResetBindings(false);
                    }
                    catch (Exception)
                    {
                    }
                    if (Environment.MachineName == Environment.UserDomainName)
                        cboDominio.Text = "Local";
                    else
                        cboDominio.Text = Environment.UserDomainName;
                    lnkCambiarClave.Visible = true;
                };
        }
        #endregion

        #region Botones de Login

        private void btnLogin_Click(object sender, EventArgs e)
        {
            bool cancelarEvento = false;
            try
            {
                HelperValidaciones helperValidaciones = new HelperValidaciones(this.errorProvider1);
                helperValidaciones.ValidarCampoObligatorio((Control)txtUser, labelControl1.Text);
                helperValidaciones.ValidarCampoObligatorio((Control)txtPass, labelControl2.Text);

                if (!helperValidaciones.EsValido())
                {
                    cancelarEvento = true;
                    return;
                }

                Cursor.Current = Cursors.WaitCursor;
                SplashScreenManager.ShowForm(typeof(WaitFormSeguridad), true, true);

                if (ConfigurationManager.AppSettings["UrlSeguridad"] != null)
                    LoginRemoto();
                else
                    LoginLocal();

                DialogResult = System.Windows.Forms.DialogResult.OK;
            }
            catch (DataPortalException ex)
            {
                if (ex.BusinessException.GetType() == typeof(UsuarioNoActivoException)
                    || ex.BusinessException.GetType() == typeof(UsuarioSinClaveException))
                    CargaActivacionUsuario(ex.BusinessException.Message);
                else
                {
                    XtraMessageBox.Show(
                        ex.BusinessException.Message,
                        Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    RegistrarLog(ex);
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg = ex.InnerException.Message;

                XtraMessageBox.Show(
                    msg,
                    Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                RegistrarLog(ex);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                if (cancelarEvento == false)
                    SplashScreenManager.CloseForm();
            }
        }

        private static void RegistrarLog(Exception ex)
        {
            try
            {
                System.Diagnostics.Trace.WriteLine(ex.ToString());
            }
            catch
            {
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if (FormAlone)
                Application.Exit();
            else
                DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
        #endregion

        #region Cambio de Clave
        private void lnkCambiarClave_OpenLink(object sender, DevExpress.XtraEditors.Controls.OpenLinkEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                using (var frm = new CambioClave(CambiarClave.GetCambiarClave(
                    new FiltroUsuarios
                    {
                        Usuario = txtUser.Text,
                        Dominio = cboDominio.Text
                    })))
                {
                    if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        XtraMessageBox.Show(
                            "Contraseña cambiada con éxito!",
                            Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        txtPass.Focus();
                    }
                }
            }
            catch (DataPortalException ex)
            {
                XtraMessageBox.Show(
                    ex.BusinessException.Message,
                    Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    ex.Message,
                    Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }
        #endregion

        #region Metodos Privados
        private void CargaActivacionUsuario(string msg)
        {
            XtraMessageBox.Show(
                                msg,
                                Text,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            using (var frm = new ActivacionUsuario(
                ActivarUsuario.GetActivarUsuario(new FiltroUsuarios
                {
                    Usuario = txtUser.Text,
                    Dominio = cboDominio.Text
                })))
            {
                frm.ShowDialog();
            }
        }

        private void LoginLocal()
        {
            //Comprobamos que el Nombre de Usuario contiene un Dominio
            var infoUser = Publicos.ComprobarDominioEnUsuario(txtUser.Text);
            infoUser.Dominio = cboDominio.Text;

            var result = false;

            //Comprobamos primero el tipo de Usuario
            if (GRPrincipal.Load(infoUser.Usuario, infoUser.Dominio))
            {
                InfoUsuario.Initialize();
                if (InfoUsuario.Instancia.Tipo)
                    result = GRPrincipal.Login(infoUser.Usuario, txtPass.Text, cboDominio.Text);
                else
                    result = GRPrincipal.Login(infoUser.Usuario, GRCrypto.Encriptar(txtPass.Text));
            }
            else
                throw new InvalidOperationException("El usuario no está inscrito para este Sistema");

            if (!result)
                throw new SecurityException("El usuario o clave no son válidos!");
        }

        private void LoginRemoto()
        {
            using (var proxy = new ProxySeguridad())
            {
                var cryptocon = new SimpleInteroperableEncryption();

                var respuesta = proxy.Login(new RequestLogin
                {
                    AcronimoAplicacion = ConfigurationManager.AppSettings["AcronimoAplicacion"],
                    CodigoUsuario = txtUser.Text,
                    Clave = cryptocon.Encrypt(txtPass.Text),
                    Dominio = cboDominio.EditValue.ToString(),
                });

                if (!respuesta.Resultado.Success)
                    throw new SecurityException(respuesta.Resultado.Message);

                InfoUsuario.Initialize();
                InfoUsuario.Instancia.IdPerfilUsuario = respuesta.IdPerfilUsuario;

                var response = proxy.GetInfoUsuario(new RequestInfoUsuario
                {
                    IdPerfilUsuario = InfoUsuario.Instancia.IdPerfilUsuario
                });

                InfoUsuario.Instancia.NombresCompletos = response.NombresCompletos;
                InfoUsuario.Instancia.CorreoUsuario = response.Correo;
                InfoUsuario.Instancia.Dominio = response.Dominio;
                InfoUsuario.Instancia.FechaUltimoIngreso = DateTime.Today;
            }
        }
        #endregion

    }
}