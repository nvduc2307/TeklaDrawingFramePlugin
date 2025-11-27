using FrameDrawingPlugin.Actions;
using System.Collections.Generic;
using System.Linq;
using Tekla.Structures.Plugins;
using tsm = Tekla.Structures.Model;
using tsmui = Tekla.Structures.Model.UI;
using tsd = Tekla.Structures.Drawing;
using FrameDrawingPlugin.Utils.Messages;

namespace FrameDrawingPlugin
{
    [Plugin("Frame Drawing")]
    [PluginUserInterface("DirectionFloorPlugin.FrameDrawingPluginView")]
    public class FrameDrawingPlugin : PluginBase
    {
        private string _assName = "Rafter";
        private tsm.Model _model;
        private tsmui.Picker _picker;
        private tsd.DrawingHandler _dHandler;
        private FrameDrawingPluginAction _action;
        public FrameDrawingPlugin()
        {
            _model = new tsm.Model();
            _picker = new tsmui.Picker();
            _dHandler = new tsd.DrawingHandler();
            _action = new FrameDrawingPluginAction(_model, _dHandler);
        }
        public override List<InputDefinition> DefineInput()
        {
            var result = new List<InputDefinition>();
            var isDo = true;
            do
            {
                try
                {
                    var obj = _picker.PickObject(tsmui.Picker.PickObjectEnum.PICK_ONE_OBJECT, "Pick a Assembly");
                    if (!(obj is tsm.Assembly assembly))
                    {
                        IO.ShowWarning("Obj must be a Assembly");
                        continue;
                    }
                    if (assembly.Name != _assName)
                    {
                        IO.ShowWarning("Assembly Name must be a Rafter");
                        continue;
                    }
                    result.Add(new InputDefinition(obj.Identifier));
                    isDo = false;
                }
                catch (System.Exception)
                {
                    isDo = false;
                }
            } while (isDo);
            return result;
        }

        public override bool Run(List<InputDefinition> inputs)
        {
            if (!inputs.Any())
                return false;
            try
            {
                var id = inputs[0].GetInput();
                var ob = _model.SelectModelObject(id as Tekla.Structures.Identifier);
                if (!(ob is tsm.Assembly frame)) throw new System.Exception("Object is not a Assembly");
                _action.Execute(frame);
            }
            catch (System.Exception ex)
            {
                IO.ShowWarning(ex.Message);
            }
            return false;
        }
    }
}
