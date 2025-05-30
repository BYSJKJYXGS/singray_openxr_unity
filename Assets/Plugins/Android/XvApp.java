package com.xv;


import android.app.Application;

import com.dongao.dlna.DLNALibrary;
import com.dongao.dlna.upnp.UpnpServiceBiz;

import com.iflytek.cloud.SpeechConstant;
import com.iflytek.cloud.SpeechUtility;

public class XvApp extends Application {

    private static final String App_ID = "60408e1a";

    @Override
    public void onCreate() {
		initAlitalk();
        super.onCreate();
		DLNALibrary.getInstance().init(this);
    }


	@Override
    public void onTerminate() {
        UpnpServiceBiz.newInstance().closeUpnpService(this);
        super.onTerminate();
    }
	
	private void initAlitalk() {
		// 应用程序入口处调用,避免手机内存过小,杀死后台进程后通过历史intent进入Activity造成SpeechUtility对象为null
        // 注意：此接口在非主进程调用会返回null对象，如需在非主进程使用语音功能，请增加参数：SpeechConstant.FORCE_LOGIN+"=true"
        // 参数间使用“,”分隔。
        // 设置你申请的应用appid

        // 注意： appid 必须和下载的SDK保持一致，否则会出现10407错误

        StringBuffer param = new StringBuffer();
        param.append("appid=" + App_ID);
        param.append(",");
        // 设置使用v5+
        param.append(SpeechConstant.ENGINE_MODE + "=" + SpeechConstant.MODE_MSC);
        SpeechUtility.createUtility(XvApp.this, param.toString());
	}

}
