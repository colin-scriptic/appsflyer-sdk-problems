using System;
using AppsFlyerConnector;
using AppsFlyerSDK;
using UnityEngine;
using UnityEngine.Assertions;

public class AppsFlyerInitialiser : MonoBehaviour, IAppsFlyerPurchaseValidation
{
        [SerializeField] private string AndroidAppKey = "";

        [SerializeField] private int FramesToWait = 30;

        private int _waitedFramesUntilInitialise = 0;
        private bool _initialised = false;
        
        private void Update()
        {
                if (!_initialised)
                {
                        Debug.Log("Waiting...");
                        if (_waitedFramesUntilInitialise < FramesToWait)
                        {
                                _waitedFramesUntilInitialise++;
                        }
                        else
                        {
                                Debug.Log("Initialising...");
                                _initialised = true;
                                Initialise();
                        }
                }
        }

        private void Initialise()
        {
#if UNITY_EDITOR
                Debug.Log("Editor not supported");
#else
                Debug.Log("Setting customer UserID");
                AppsFlyer.setCustomerUserId("footest");
                Debug.Log("Setting enableTCFDataCollection");
                AppsFlyer.enableTCFDataCollection(true);

        #if UNITY_ANDROID
                Debug.Log("Initialising SDK");
                AppsFlyer.initSDK(AndroidAppKey, "", this);
                Assert.IsNotNull(AppsFlyer.instance);
                Assert.IsTrue(AppsFlyer.instance.isInit);
                Assert.IsNotNull(FindObjectOfType<AppsFlyerPurchaseConnector>());
                Debug.Log("SDK is definitely initialised and AppsFlyerPurchaseConnector exists");
        #else
                Debug.LogError("Unsupported platform");
                return;
        #endif
                Debug.Log("Try to init AppsFlyerPurchaseConnector");
                AppsFlyerPurchaseConnector.init(this, AppsFlyerConnector.Store.GOOGLE);
                Debug.Log("Initialised AppsFlyerPurchaseConnector");
#if DEBUG
                AppsFlyerPurchaseConnector.setIsSandbox(true);
#endif
                AppsFlyerPurchaseConnector.setAutoLogPurchaseRevenue(AppsFlyerAutoLogPurchaseRevenueOptions.AppsFlyerAutoLogPurchaseRevenueOptionsInAppPurchases);
                AppsFlyerPurchaseConnector.setPurchaseRevenueValidationListeners(true);
                AppsFlyerPurchaseConnector.build();
                AppsFlyerPurchaseConnector.startObservingTransactions();
                
                AppsFlyer.startSDK();
#endif
        }

        public void didReceivePurchaseRevenueValidationInfo(string validationInfo)
        {
                AppsFlyer.AFLog("didReceivePurchaseRevenueValidationInfo", validationInfo);
        }

        public void didReceivePurchaseRevenueError(string error)
        {
                Debug.LogError($"didReceivePurchaseRevenueError: {error}");
        }
}
