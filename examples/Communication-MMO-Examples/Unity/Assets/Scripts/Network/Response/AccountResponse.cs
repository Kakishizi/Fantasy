using UnityEngine;
using Fantasy;

public partial class Response : MonoBehaviour
{
    RegisterPanel reg;
    LoginPanel login;

    public async void RegisterResponse(uint Error)
    {
        if(reg ==null)
            reg = UIFacade.Instance.GetUIPanel(StringManager.RegisterPanel) as RegisterPanel;

        // 密码长度不够
        if(Error == ErrorCode.Error_PwTooShort){
            reg.prompt.text = StringManager.PwTooShort;
            return;
        }
        // 帐号已经存在
        if (Error == ErrorCode.Error_RegisterAccountAlreayRegister)
        {
            reg.ResetPanel();
            reg.prompt.text = StringManager.AccountAlreayRegister;
            return;
        }
        
        // 注册成功
        // >>process account 返回登录界面
        if(Error == ErrorCode.Success){
            reg.prompt.text = StringManager.RegSuced;

            await TimerScheduler.Instance.Core.WaitAsync(3000);
            
            reg.BackToPanel();
        }     
    }

    public void LoginResponse(uint Error)
    {
        if(login ==null)
            login = UIFacade.Instance.GetUIPanel(StringManager.LoginPanel) as LoginPanel;

        if(Error == ErrorCode.Error_LoginAccountNotRegister)
        {
            login.ResetPanel();
            login.prompt.text = StringManager.AccountNotRegister;
            return;
        }

        // 密码长度不够
        if(Error == ErrorCode.Error_PwTooShort){
            login.prompt.text = StringManager.PwTooShort;
            return;
        }

        // 判断realm验证结果
        if (Error == ErrorCode.Error_LoginAccountPwError)
        {
            login.ResetPanel();
            login.prompt.text = StringManager.LoginFailed;
            return;
        }
        
        // 登录帐号成功
        if(Error == ErrorCode.Success){
            login.prompt.text = StringManager.LoginSuced;

            // >>process role 从登录进入选角界面
            UIFacade.Instance.EnterScene(new RoleScene());
        }
        
    }
}