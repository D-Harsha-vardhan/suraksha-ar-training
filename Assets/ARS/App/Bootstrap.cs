using ARS.Core;
using ARS.Core.Localization;
using ARS.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ARS.App
{
    public sealed class Bootstrap : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Create()
        {
            if (FindFirstObjectByType<Bootstrap>() == null) new GameObject("ARashak Bootstrap").AddComponent<Bootstrap>();
        }

        private Canvas canvas;
        private string workerName = "Trainee";
        private string workerId = "";
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            DisableTemplateOverlay();
            Loc.LoadJson("en", "{\"app.tagline\":\"Train safe. Work safe. Go home safe.\",\"action.continue\":\"Continue\",\"home.title\":\"Training modules\",\"module.fire\":\"Fire & Explosion Response\"}");
            ShowSplash();
        }

        private static void DisableTemplateOverlay()
        {
            // The imported AR Mobile Template ships with a full-screen GreetingCTA
            // modal that intercepts all taps. This app supplies its own UI instead.
            foreach (var candidate in FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (candidate.gameObject.name != "ARashak UI") candidate.gameObject.SetActive(false);
            }
        }

        private void ResetScreen(bool transparentBackground = false)
        {
            if (canvas != null) Destroy(canvas.gameObject);
            canvas = UIBuilder.Canvas();
            var color = Theme.Bg;
            if (transparentBackground) color.a = 0f;
            var background = UIBuilder.Panel(canvas.transform, "Background", color);
            UIBuilder.Stretch(background.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        }

        private TextMeshProUGUI Label(string text, float size, Color color, Vector2 min, Vector2 max, TextAlignmentOptions alignment = TextAlignmentOptions.Left)
        {
            var label = UIBuilder.Text(canvas.transform, text, size, color);
            label.alignment = alignment;
            UIBuilder.Stretch(label.rectTransform, min, max, Vector2.zero, Vector2.zero);
            return label;
        }

        private Image Card(string name, Vector2 min, Vector2 max, Color color)
        {
            var card = UIBuilder.Panel(canvas.transform, name, color);
            UIBuilder.Stretch(card.rectTransform, min, max, Vector2.zero, Vector2.zero);
            return card;
        }

        private void AddCardText(Transform parent, string title, string detail, string status, Color statusColor)
        {
            var heading = UIBuilder.Text(parent, title, 34, Theme.Text);
            UIBuilder.Stretch(heading.rectTransform, new Vector2(.07f, .55f), new Vector2(.93f, .87f), Vector2.zero, Vector2.zero);
            var description = UIBuilder.Text(parent, detail, 23, Theme.Muted);
            UIBuilder.Stretch(description.rectTransform, new Vector2(.07f, .2f), new Vector2(.7f, .53f), Vector2.zero, Vector2.zero);
            var pill = UIBuilder.Text(parent, status, 20, statusColor);
            pill.alignment = TextAlignmentOptions.Right;
            UIBuilder.Stretch(pill.rectTransform, new Vector2(.62f, .2f), new Vector2(.93f, .53f), Vector2.zero, Vector2.zero);
        }

        private void ShowSplash()
        {
            ResetScreen();
            var crest = Label("▣", 126, Theme.Blue, new Vector2(.35f, .67f), new Vector2(.65f, .82f), TextAlignmentOptions.Center);
            var title = Label(AppConfig.AppName, 74, Theme.Text, new Vector2(.08f, .57f), new Vector2(.92f, .67f), TextAlignmentOptions.Center);
            title.fontStyle = FontStyles.Bold;
            Label(Loc.T("app.tagline"), 29, Theme.Muted, new Vector2(.1f, .5f), new Vector2(.9f, .56f), TextAlignmentOptions.Center);
            var demo = Label("DEMO TRAINING SIMULATOR", 19, Theme.Amber, new Vector2(.1f, .42f), new Vector2(.9f, .47f), TextAlignmentOptions.Center);
            demo.characterSpacing = 3;
            var button = UIBuilder.PrimaryButton(canvas.transform, "Continue", ShowSignIn);
            UIBuilder.Stretch(button.GetComponent<RectTransform>(), new Vector2(.1f, .12f), new Vector2(.9f, .2f), Vector2.zero, Vector2.zero);
        }

        private void ShowSignIn()
        {
            ResetScreen();
            Label("WELCOME TO", 27, Theme.Green, new Vector2(.1f, .86f), new Vector2(.9f, .91f));
            var title = Label(AppConfig.AppName, 64, Theme.Text, new Vector2(.1f, .77f), new Vector2(.9f, .86f)); title.fontStyle = FontStyles.Bold;
            Label("Enter your details to personalize safety training", 30, Theme.Muted, new Vector2(.1f, .69f), new Vector2(.9f, .75f));
            var worker = UIBuilder.Input(canvas.transform, "Worker ID (e.g. JHAR-1024)"); UIBuilder.Stretch(worker.GetComponent<RectTransform>(), new Vector2(.1f, .56f), new Vector2(.9f, .64f), Vector2.zero, Vector2.zero);
            var name = UIBuilder.Input(canvas.transform, "Full name"); UIBuilder.Stretch(name.GetComponent<RectTransform>(), new Vector2(.1f, .45f), new Vector2(.9f, .53f), Vector2.zero, Vector2.zero);
            var continueButton = UIBuilder.PrimaryButton(canvas.transform, "Continue to training", () => { workerId = worker.text.Trim(); workerName = string.IsNullOrWhiteSpace(name.text) ? "Trainee" : name.text.Trim(); ShowHome(); });
            UIBuilder.Stretch(continueButton.GetComponent<RectTransform>(), new Vector2(.1f, .27f), new Vector2(.9f, .35f), Vector2.zero, Vector2.zero);
            Label("Your details stay on this device · Works offline", 23, Theme.Muted, new Vector2(.1f, .2f), new Vector2(.9f, .25f), TextAlignmentOptions.Center);
        }

        private void ShowHome()
        {
            ResetScreen();
            Label("ARashak", 38, Theme.Text, new Vector2(.07f, .91f), new Vector2(.55f, .97f));
            Label("EN   ◉ OFFLINE", 19, Theme.Muted, new Vector2(.57f, .915f), new Vector2(.93f, .97f), TextAlignmentOptions.Right);
            Label($"Hi, {workerName}", 38, Theme.Text, new Vector2(.07f, .84f), new Vector2(.93f, .9f));
            if (!string.IsNullOrWhiteSpace(workerId)) Label($"Worker ID: {workerId}", 22, Theme.Muted, new Vector2(.07f, .81f), new Vector2(.93f, .84f));

            var readiness = Card("Readiness", new Vector2(.07f, .69f), new Vector2(.93f, .81f), Theme.BlueSoft);
            var percent = UIBuilder.Text(readiness.transform, "0%", 45, Theme.Green); UIBuilder.Stretch(percent.rectTransform, new Vector2(.07f,.25f), new Vector2(.26f,.8f), Vector2.zero, Vector2.zero);
            var readyText = UIBuilder.Text(readiness.transform, "Safety readiness\nStart your first module", 25, Theme.Text); UIBuilder.Stretch(readyText.rectTransform, new Vector2(.3f,.2f), new Vector2(.92f,.82f), Vector2.zero, Vector2.zero);
            Label("TRAINING MODULES", 25, Theme.Muted, new Vector2(.07f, .64f), new Vector2(.93f, .68f));

            var fire = Card("FireModule", new Vector2(.07f, .47f), new Vector2(.93f, .61f), Theme.Surface);
            AddCardText(fire.transform, "🔥  Fire & Explosion Response", "6 min · Practice response actions", "START", Theme.Green);
            var fireButton = fire.gameObject.AddComponent<Button>(); fireButton.onClick.AddListener(ShowFireBrief);
            var gas = Card("GasModule", new Vector2(.07f, .30f), new Vector2(.93f, .44f), Theme.Surface2);
            AddCardText(gas.transform, "☣  Gas Leak & Confined Space", "8 min · Practice safe isolation", "START", Theme.Green); gas.gameObject.AddComponent<Button>().onClick.AddListener(ShowFireBrief);
            var ppe = Card("PpeModule", new Vector2(.07f, .13f), new Vector2(.93f, .27f), Theme.Surface2);
            AddCardText(ppe.transform, "⛑  Machinery & PPE", "5 min · PPE safety practice", "START", Theme.Green); ppe.gameObject.AddComponent<Button>().onClick.AddListener(ShowFireBrief);
            Label("⌂ Home                 ◴ Progress                 ⚙ Settings", 20, Theme.Muted, new Vector2(.06f, .04f), new Vector2(.94f, .09f), TextAlignmentOptions.Center);
        }

        private void ShowFireBrief()
        {
            ResetScreen();
            Label("FIRE RESPONSE", 23, Theme.Amber, new Vector2(.08f, .9f), new Vector2(.92f, .95f), TextAlignmentOptions.Center);
            var title = Label("Fire & Explosion\nResponse", 52, Theme.Text, new Vector2(.08f, .69f), new Vector2(.92f, .84f), TextAlignmentOptions.Center); title.fontStyle = FontStyles.Bold;
            Label("Training simulation only. In a real emergency, follow your site procedure, raise the alarm, and evacuate if unsafe.", 27, Theme.Muted, new Vector2(.1f, .48f), new Vector2(.9f, .63f), TextAlignmentOptions.Center);
            var start = UIBuilder.PrimaryButton(canvas.transform, "Start AR training", ShowArTraining);
            UIBuilder.Stretch(start.GetComponent<RectTransform>(), new Vector2(.1f, .18f), new Vector2(.9f, .26f), Vector2.zero, Vector2.zero);
        }

        private void ShowArTraining()
        {
            DisableTemplateOverlay();
            // AR camera video must remain visible behind this overlay.
            ResetScreen(true);
            Label("FIRE RESPONSE                                      1 / 6", 28, Theme.Text, new Vector2(.06f, .91f), new Vector2(.94f, .96f));
            var instruction = Label("SCAN A FLOOR, THEN TAP TO PLACE", 42, Theme.Text, new Vector2(.08f, .5f), new Vector2(.92f, .58f), TextAlignmentOptions.Center);
            Label("Move your phone slowly over a well-lit floor. Tap the detected surface to place the extinguisher training model.", 30, Theme.Muted, new Vector2(.1f, .4f), new Vector2(.9f, .49f), TextAlignmentOptions.Center);
            var objective = Card("Objective", new Vector2(.06f, .1f), new Vector2(.94f, .25f), new Color(Theme.Surface.r, Theme.Surface.g, Theme.Surface.b, .94f));
            var objectiveText = UIBuilder.Text(objective.transform, "●  Scan the floor to place the training scenario", 34, Theme.Text); UIBuilder.Stretch(objectiveText.rectTransform, new Vector2(.06f,.2f), new Vector2(.94f,.8f), Vector2.zero, Vector2.zero);
            var placement = FindFirstObjectByType<ARTrainingPlacement>();
            if (placement == null) placement = new GameObject("AR Training Placement").AddComponent<ARTrainingPlacement>();
            placement.OnPlacementChanged = placed => { if (placed) instruction.text = "MODEL PLACED · TAP AGAIN TO MOVE IT"; };
        }
    }
}
