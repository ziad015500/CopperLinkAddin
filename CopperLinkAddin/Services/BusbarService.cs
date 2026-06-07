using System;
using System.IO;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using CopperLinkAddin.Models;

namespace CopperLinkAddin.Services
{
    public class BusbarService
    {
        private ISldWorks iSwApp;

        public BusbarService(ISldWorks swApp)
        {
            iSwApp = swApp;
        }

        public void CreateBusbar(BusbarModel model)
        {
            int longstatus = 0;

            ModelDoc2 part = (ModelDoc2)iSwApp.NewDocument(
                @"C:\ProgramData\SolidWorks\SOLIDWORKS 2025\templates\Part.prtdot",
                0, 0, 0);

            iSwApp.ActivateDoc2("Part1", false, ref longstatus);
            part = (ModelDoc2)iSwApp.ActiveDoc;

            double thickness = model.Thickness / 1000.0;
            double width = model.Width / 1000.0;
            double bendRadius = model.BendRadius / 1000.0;
            double d1 = model.D1 / 1000.0;
            double d2 = model.D2 / 1000.0;
            double d3 = model.D3 / 1000.0;

            if (model.ShapeType == 2 || model.ShapeType == 3)
            {
                d1 -= thickness;
                d2 -= thickness;
            }
            else if (model.ShapeType == 4 || model.ShapeType == 5)
            {
                d1 -= width;
                d2 -= width;
            }

            string planeName = (model.ShapeType == 4 || model.ShapeType == 5)
                ? "Front Plane"
                : "Top Plane";

            part.Extension.SelectByID2(
                planeName, "PLANE", 0, 0, 0, false, 0, null, 0);

            SketchManager skMgr = part.SketchManager;
            skMgr.InsertSketch(true);

            if (model.ShapeType == 1)
            {
                skMgr.CreateLine(0, 0, 0, d1, 0, 0);
            }
            else if (model.ShapeType == 2 || model.ShapeType == 4)
            {
                skMgr.CreateLine(0, 0, 0, d1, 0, 0);
                skMgr.CreateLine(d1, 0, 0, d1, d2, 0);
            }
            else if (model.ShapeType == 3 || model.ShapeType == 5)
            {
                skMgr.CreateLine(0, 0, 0, d1, 0, 0);
                skMgr.CreateLine(d1, 0, 0, d1, d2, 0);
                skMgr.CreateLine(d1, d2, 0, d1 + d3, d2, 0);
            }

            skMgr.InsertSketch(true);
            part.ClearSelection2(true);

            Feature sketchFeat = (Feature)part.FirstFeature();
            while (sketchFeat != null && sketchFeat.GetTypeName2() != "ProfileFeature")
                sketchFeat = (Feature)sketchFeat.GetNextFeature();
            string sketchName = sketchFeat.Name;

            part.Extension.SelectByID2(
                sketchName, "SKETCH", 0, 0, 0, false, 0, null, 0);

            FeatureManager featMgr = part.FeatureManager;

            if (model.ShapeType == 1 || model.ShapeType == 4 || model.ShapeType == 5)
            {
                featMgr.FeatureExtrusionThin2(
                    true, false, false,
                    0, 0,
                    thickness,
                    0.01,
                    false, false, false, false,
                    1.74532925199433E-02,
                    1.74532925199433E-02,
                    false, false, false, false,
                    true,
                    width,
                    0.01, 0.01,
                    0, 0,
                    false,
                    0.005,
                    true, true,
                    0, 0, false);
            }
            else if (model.ShapeType == 2 || model.ShapeType == 3)
            {
                object customBendData = featMgr.CreateCustomBendAllowance();
                ((CustomBendAllowance)customBendData).KFactor = 0.5;

                BaseFlangeFeatureData featData =
                    (BaseFlangeFeatureData)featMgr.CreateDefinition(
                        (int)swFeatureNameID_e.swFmBaseFlange);

                featData.Initialize(
                    false, true, customBendData,
                    true, 1, true, 0.5, 0.0001, 0.0001);

                featData.BendRadius = bendRadius;
                featData.D1EndConditionDistance = width;
                featData.D1EndConditionType = 1;
                featData.D2EndConditionDistance = thickness;
                featData.D2EndConditionType = 1;
                featData.OffsetDirections = 1;
                featData.ReverseDirection = false;
                featData.ReverseThickness = false;
                featData.Thickness = thickness;

                featMgr.CreateFeature(featData);
            }

            // Apply Material
            string dbPath = @"C:\Program Files\SOLIDWORKS Corp\SOLIDWORKS\lang\english\sldmaterials\SOLIDWORKS Materials.sldmat";
            PartDoc swPart = (PartDoc)part;

            if (model.Material == "CU")
                swPart.SetMaterialPropertyName2(default, dbPath, "Copper");
            else
                swPart.SetMaterialPropertyName2(default, dbPath, "6061 Alloy");

            // Save Part
            string fullPath = Path.Combine(model.SavePath, model.FileName + ".SLDPRT");
            part.SaveAs(fullPath);

            part.ViewZoomtofit2();
        }
    }
}