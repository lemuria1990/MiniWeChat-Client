﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using protocol;

namespace MiniWeChat
{
    public class PersonalPanel : BasePanel
    {
        public enum PersonalSetType
        {
            NAME = 0,
            PASSWORD,
            HEAD,
        }

        public Text _laeblName;
        public Text _labelId;
        public Image _imageHead;

        public Button _buttonSetName;
        public Button _buttonSetPassword;
        public Button _buttonSetHead;
        public Button _buttonExit;

        private PersonalSetType _personalSetType;

        public void Start()
        {
            _buttonSetPassword.onClick.AddListener(OnClickSetPassword);
            _buttonSetName.onClick.AddListener(OnClickSetName);
            _buttonSetHead.onClick.AddListener(OnClickSetHead);
            _buttonExit.onClick.AddListener(OnClickExitButton);
        }

        public override void Show(object param = null)
        {
            base.Show(param);


            MessageDispatcher.GetInstance().RegisterMessageHandler((uint)EUIMessage.UPDATE_PERSONAL_DETAIL, OnUpdatePersonalDetail);

            _laeblName.text = GlobalUser.GetInstance().UserName;
            _labelId.text = GlobalUser.GetInstance().UserId;
            UIManager.GetInstance().SetImage(_imageHead, EAtlasName.Head, "00" + GlobalUser.GetInstance().HeadIndex);
        }

        public override void Hide()
        {
            base.Hide();
            MessageDispatcher.GetInstance().UnRegisterMessageHandler((uint)EUIMessage.UPDATE_PERSONAL_DETAIL, OnUpdatePersonalDetail);


        }

        public void OnClickExitButton()
        {
            NetworkManager.GetInstance().SendPacket<LogoutReq>(ENetworkMessage.LOGOUT_REQ, new LogoutReq());
        }

        public void OnClickSetName()
        {
            DialogManager.GetInstance().CreateDoubleButtonInputDialog("修改昵称", "昵称", "长度不能超过6", GlobalUser.GetInstance().UserName, InputField.ContentType.Standard, OnConfirmChange);
            _personalSetType = PersonalSetType.NAME;
        }

        public void OnClickSetPassword()
        {
            DialogManager.GetInstance().CreateDoubleButtonInputDialog("修改密码", "密码", "长度不能超过20", GlobalUser.GetInstance().UserPassword, InputField.ContentType.Password, OnConfirmChange);
            _personalSetType = PersonalSetType.PASSWORD;
        }

        public void OnClickSetHead()
        {
            StateManager.GetInstance().PushState<ImageListPanel>(EUIType.ImageListPanel,
                new CallBackWithString { callback = OnConfirmChange });
            _personalSetType = PersonalSetType.HEAD;
        }

        public void OnConfirmChange(string text)
        {
            PersonalSettingsReq req = new PersonalSettingsReq();

            if (_personalSetType == PersonalSetType.PASSWORD)
            {
                req.userPassword = text;
            }
            else if (_personalSetType == PersonalSetType.NAME)
            {
                req.userName = text;
            }
            else if (_personalSetType == PersonalSetType.HEAD)
            {
                req.headIndex = int.Parse(text);
            }

            NetworkManager.GetInstance().SendPacket<PersonalSettingsReq>(ENetworkMessage.PERSONALSETTINGS_REQ, req);
        }

        public void OnUpdatePersonalDetail(uint iMessageType, object kParam)
        {
            _laeblName.text = GlobalUser.GetInstance().UserName;
            UIManager.GetInstance().SetImage(_imageHead, EAtlasName.Head, "00" + GlobalUser.GetInstance().HeadIndex);
        }


    }
}

