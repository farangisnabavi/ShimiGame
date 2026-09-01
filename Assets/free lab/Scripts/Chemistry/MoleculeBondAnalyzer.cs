using System.Collections.Generic;
using UnityEngine;
using PeriodicTableSystem.Data;
using PeriodicTableSystem.Database;

namespace PeriodicTableSystem.Chemistry
{
    /// <summary>
    /// نوع پیوند شیمیایی تخمینی بین دو عنصر.
    /// این یک مدل ساده‌شده برای گیم‌پلی است، نه شبیه‌سازی دقیق کوانتومی.
    /// </summary>
    public enum BondTypeAnalyze
    {
        None,       // پیوندی تشکیل نمی‌شود (مثلاً گازهای نجیب یا ظرفیت صفر)
        Covalent,   // پیوند اشتراکی (معمولاً نافلز + نافلز)
        Ionic,      // پیوند یونی (معمولاً فلز + نافلز با اختلاف الکترونگاتیوی زیاد)
        Metallic    // پیوند فلزی / آلیاژ (فلز + فلز)
    }

    /// <summary>
    /// نتیجه‌ی بررسی پیوند بین دو عنصر، شامل نوع پیوند و فرمول تخمینی مولکول.
    /// </summary>
    [System.Serializable]
    public class BondResultAnalyze
    {
        public PeriodicElementData elementA;
        public PeriodicElementData elementB;
        public BondTypeAnalyze BondTypeAnalyze;

        // تعداد تخمینی اتم‌های هر عنصر در ساده‌ترین فرمول مولکولی (مثل AxBy)
        public int countA;
        public int countB;

        public float electronegativityDifference;

        public string GetFormula()
        {
            if (BondTypeAnalyze == BondTypeAnalyze.None) return "-";

            string symA = elementA.symbol;
            string symB = elementB.symbol;

            string partA = countA > 1 ? $"{symA}{countA}" : symA;
            string partB = countB > 1 ? $"{symB}{countB}" : symB;

            return partA + partB;
        }

        public override string ToString()
        {
            return $"{elementA.elementName} + {elementB.elementName} => " +
                   $"[{BondTypeAnalyze}] تخمین فرمول: {GetFormula()} " +
                   $"(ΔEN = {electronegativityDifference:F2})";
        }
    }

    /// <summary>
    /// تحلیل‌گر پیوند بین تمام عناصر یک لیست (مثلاً همه‌ی عناصر موجود در آزمایشگاه آزاد).
    /// طراحی شده مطابق معماری فعلی پروژه: بدون Singleton، رفرنس‌ها از طریق Inspector.
    /// </summary>
    public class MoleculeBondAnalyzer : MonoBehaviour
    {
        [Header("Data Source")]
        [Tooltip("دیتابیس تمام عناصری که در آزمایشگاه آزاد وجود دارند (همون AllElements.asset)")]
        [SerializeField] private ElementDatabase elementDatabase;

        [Header("Bonding Rules")]
        [Tooltip("اختلاف الکترونگاتیوی بالاتر از این مقدار، پیوند را یونی در نظر می‌گیرد")]
        [Range(0.5f, 3.0f)]
        [SerializeField] private float ionicThreshold = 1.7f;

        [Header("Result (Read-Only)")]
        [SerializeField] private List<BondResultAnalyze> possibleBonds = new List<BondResultAnalyze>();

        public IReadOnlyList<BondResultAnalyze> PossibleBonds => possibleBonds;

        /// <summary>
        /// نقطه ورودی اصلی: تمام جفت‌های ممکن از عناصر لیست را بررسی می‌کند
        /// (شامل بررسی پیوند یک عنصر با خودش، مثل O2 یا N2)
        /// </summary>
        [ContextMenu("Analyze All Elements")]
        public void AnalyzeAllElements()
        {
            if (elementDatabase == null)
            {
                Debug.LogWarning("[MoleculeBondAnalyzer] ElementDatabase متصل نشده است. آن را در Inspector درگ کنید.");
                return;
            }

            var allElements = elementDatabase.Elements;
            possibleBonds.Clear();

            for (int i = 0; i < allElements.Count; i++)
            {
                for (int j = i; j < allElements.Count; j++) // j = i برای شامل کردن پیوند با خود (دی‌اتمی)
                {
                    var a = allElements[i];
                    var b = allElements[j];

                    if (a == null || b == null) continue;

                    BondResultAnalyze result = EvaluateBond(a, b);
                    if (result.BondTypeAnalyze != BondTypeAnalyze.None)
                    {
                        possibleBonds.Add(result);
                    }
                }
            }

            Debug.Log($"[MoleculeBondAnalyzer] بررسی {allElements.Count} عنصر انجام شد. " +
                      $"{possibleBonds.Count} پیوند ممکن یافت شد.");
        }

        /// <summary>
        /// بررسی پیوند بین دقیقاً دو عنصر مشخص (برای فراخوانی مستقیم هنگام drag & drop اتم‌ها روی هم در بازی)
        /// </summary>
        public BondResultAnalyze EvaluateBond(PeriodicElementData a, PeriodicElementData b)
        {
            var result = new BondResultAnalyze
            {
                elementA = a,
                elementB = b,
                electronegativityDifference = Mathf.Abs(a.electronegativity - b.electronegativity)
            };

            // قدم ۱: اگر هیچ‌کدام ظرفیت پیوند ندارند (مثل گازهای نجیب با maxBonds = 0) -> پیوندی نیست
            if (a.maxBonds <= 0 || b.maxBonds <= 0)
            {
                result.BondTypeAnalyze = BondTypeAnalyze.None;
                return result;
            }

            // قدم ۲: تشخیص نوع پیوند
            if (a.isMetal && b.isMetal)
            {
                // فلز + فلز -> پیوند فلزی (آلیاژ). این‌ها معمولاً "مولکول" کلاسیک نمی‌سازند
                // ولی برای گیم‌پلی می‌توان به عنوان ترکیب فلزی در نظر گرفت.
                result.BondTypeAnalyze = BondTypeAnalyze.Metallic;
            }
            else if (!a.isMetal && !b.isMetal)
            {
                // نافلز + نافلز -> معمولاً کووالانسی، مگر اختلاف الکترونگاتیوی خیلی زیاد باشد
                result.BondTypeAnalyze = result.electronegativityDifference >= ionicThreshold
                    ? BondTypeAnalyze.Ionic
                    : BondTypeAnalyze.Covalent;
            }
            else
            {
                // یکی فلز و دیگری نافلز -> بسته به اختلاف الکترونگاتیوی
                result.BondTypeAnalyze = result.electronegativityDifference >= ionicThreshold
                    ? BondTypeAnalyze.Ionic
                    : BondTypeAnalyze.Covalent; // مثلاً برخی نیمه‌فلزها می‌توانند کووالانسی بسازند
            }

            // قدم ۳: تخمین ساده‌ترین فرمول مولکولی با روش "cross multiply" ظرفیت‌ها
            // مثال: اگر ظرفیت A = 2 و ظرفیت B = 3 -> A3B2 (مثل Al2O3 با ظرفیت آلومینیوم 3 و اکسیژن 2)
            int valenceA = Mathf.Max(1, a.maxBonds);
            int valenceB = Mathf.Max(1, b.maxBonds);

            int countA = valenceB;
            int countB = valenceA;

            int gcd = GCD(countA, countB);
            if (gcd > 0)
            {
                countA /= gcd;
                countB /= gcd;
            }

            // حالت خاص پیوند یک عنصر با خودش (دی‌اتمی مثل O2, N2, H2, Cl2)
            if (a == b)
            {
                countA = 2;
                countB = 0; // یعنی فقط از فرمول A2 استفاده شود، نه A_xB_y
            }

            result.countA = countA;
            result.countB = a == b ? 0 : countB;

            return result;
        }

        private static int GCD(int x, int y)
        {
            while (y != 0)
            {
                (x, y) = (y, x % y);
            }
            return x == 0 ? 1 : x;
        }

        /// <summary>
        /// چاپ تمام نتایج در کنسول (برای دیباگ سریع)
        /// </summary>
        [ContextMenu("Log All Bonds")]
        public void LogAllBonds()
        {
            foreach (var bond in possibleBonds)
            {
                Debug.Log(bond.ToString());
            }
        }
    }
}
