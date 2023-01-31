/**
 * The SKAdNetwork class implements a strategy for SKAdNetwork on iOS
 * Dependencies: Tenjin and IronSource
 * 
 * Usage: run SKAdNetwork.Init()
 */
using UnityEngine;
using System.Collections;
using System;

namespace LuckyKat 
{
    public static class SKAdNetwork
    {
        //private static BaseTenjin tenjinInstance;
        private static long playerStartTime = 0;

        // Return ms since epoch
        private static long GetTime()
        {
            return (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }

        // Check if player is playing for the first 24 hours
        private static bool IsPlayingFirstDay()
        {
            var now = GetTime();
            var diff = now - playerStartTime;

            return diff < 1000 * 60 * 60 * 24;
        }

        // Converts reveneue value to conversion value
        private static int RevenueToConversion(double revenue)
        {
            int value = 0;
            double[] intervals = {
                0,
                0.001694263808080542,
                0.0035954644803992534,
                0.006203602016956133,
                0.009518676417751181,
                0.013540687682784398,
                0.018269635812055787,
                0.02370552080556534,
                0.029848342663313064,
                0.036698101385298956,
                0.044254796971523014,
                0.052518429421985244,
                0.06148899873668566,
                0.07116650491562422,
                0.08155094795880095,
                0.09264232786621586,
                0.10444064463786894,
                0.11694589827376019,
                0.1301580887738896,
                0.14407721613825716,
                0.15870328036686293,
                0.17403628145970684,
                0.19007621941678893,
                0.20682309423810918,
                0.22427690592366764,
                0.24243765447346424,
                0.261305339887499,
                0.28087996216577193,
                0.30116152130828305,
                0.3221500173150323,
                0.34384545018601975,
                0.36624781992124533,
                0.38935712652070914,
                0.41317336998441107,
                0.43769655031235116,
                0.46292666750452943,
                0.4888637215609459,
                0.5155077124816005,
                0.5428586402664933,
                0.5709165049156243,
                0.5996813064289934,
                0.6291530448066006,
                0.6593317200484461,
                0.6902173321545297,
                0.7218098811248516,
                0.7541093669594116,
                0.7871157896582096,
                0.820829149221246,
                0.8552494456485207,
                0.8903766789400333,
                0.9262108490957841,
                0.962751956115773,
                1.0,
                1.573283885819753,
                2.2759109944755553,
                3.1078813259674067,
                4.069194880295309,
                5.159851657459258,
                6.379851657459258,
                7.729194880295307,
                9.207881325967406,
                10.815910994475555,
                12.553283885819754
            };
            for (int i = 0; i < intervals.Length; ++i)
            {
                var left = intervals[i];
                if (revenue > left)
                {
                    value = i + 1;
                }

            }

            if (value > 63)
            {
                value = 63;
            }
            return value;
        }

        // Must be called before IronSrc initialization!
        public static void Init()
        {
            // This entire thing only makes sense on iOS
            #if UNITY_IOS

            // Load player start time
            playerStartTime = long.Parse(PlayerPrefs.GetString("PlayerStartTime", "0"));

            if (playerStartTime == 0)
            {
                // Not played before: set start time as current time
                long startTime = GetTime();
                // Save as string
                PlayerPrefs.SetString("PlayerStartTime", startTime.ToString());

                playerStartTime = startTime;
            }

            // Prepare SKAdNetwork
            tenjinInstance = Tenjin.getInstance("TVITWX4WGSZGBSL58QGWY7XLBYXIAK4B");
            tenjinInstance.RegisterAppForAdNetworkAttribution();
            tenjinInstance.UpdateConversionValue(0);

            // catch IronSrc Impression event
            IronSourceEvents.onImpressionSuccessEvent += ImpressionSuccessEvent;

            #endif
        }

        //Disabling the IronSource event so the SDK can be safely removed from the Assets folder -VMG
        // IronSrc event
        /*public static void ImpressionSuccessEvent(IronSourceImpressionData impressionData)
        {
            Debug.Log("unity-script:  ImpressionEvent impressionData = " + impressionData);
            //Deleted reference to tenjinInstance
            if (impressionData != null && IsPlayingFirstDay()  != null)
            {
                var revenue = impressionData.revenue.GetValueOrDefault(0);

                // add to lifetime revenue
                var lifetimeRevenue = double.Parse(PlayerPrefs.GetString("lifetimeRevenue", "0"));
                lifetimeRevenue += revenue;
                PlayerPrefs.SetString("lifetimeRevenue", lifetimeRevenue.ToString());

                // convert the lifetime revenue as conversion value
                var conversionValue = RevenueToConversion(lifetimeRevenue);

                // send using SKAdNetwork (with Tenjin)
                //tenjinInstance.UpdateConversionValue(conversionValue);

                Debug.Log("unity-script:  conversionValue " + conversionValue.ToString());
            }
        }*/
    }
}
