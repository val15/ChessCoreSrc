using ChessCore.Models;
using ChessCore.Tools;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IO.Compression;

namespace ChessCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }




        [HttpPost]
        //MOVEMENT DU CPU
        public ActionResult DetailsTimer(int whiteTimeInSecond, int blackTimeInSecond)
        {
            try
            {
                MainUtils.CpuCount++;
             //   var t_ = "cpu";

                //si CpuCount est supperier à 1, on ne prend pas en compt car le cpu est
                // deja en cour de refelextion
                if (MainUtils.CpuCount > 1)
                    return null;

                //pour le timer,
                //il faut prendre l'intervale en seconde et l'ajouter à 
                //computer timer
                var startRefelectionTime = DateTime.Now;

                //Méthode nonothread
                // var engine = new Engine(MainUtils.DeepLevel, MainUtils.CPUColor, false, null);
                //  var bestNodeChess2 = engine.Search(MainUtils.VM.MainBord, MainUtils.TurnNumber.ToString(), -1, -1);
                //méthode multithreading
                using (var chess2UtilsNotStatic = new Chess2UtilsNotStatic())
                {

                    var bestNode = chess2UtilsNotStatic.GetBestPositionLocalUsingMiltiThreading(MainUtils.CPUColor, MainUtils.VM.MainBord, true, null);
                    var fromIndex = chess2UtilsNotStatic.GetIndexFromLocation(bestNode.Location);//int
                    var toIndex = chess2UtilsNotStatic.GetIndexFromLocation(bestNode.BestChildPosition);

                    //determination si attaque pour remplir le cimetiere
                    //  var destinationCase = MainUtils.VM.MainBord.GetCases()[bestNodeChess2.ToIndex];
                    //le CPU a perdu si bestNodeChess2.FromIndex == bestNodeChess2.ToIndex
                    if (fromIndex == toIndex)
                        return View("Losing");


                    MainUtils.VM.MainBord.Move(fromIndex, toIndex, 0);
                    MainUtils.TurnNumber++;
                    MainUtils.VM.Refresh(MainUtils.VM.MainBord);
                    MainUtils.FromGridIndex = -1;
                    var dateTimeNow = DateTime.Now;
                    var interval = (int)(dateTimeNow - startRefelectionTime).TotalSeconds;
                    if (Utils.ComputerColor == "W")
                        whiteTimeInSecond = interval;
                    else
                        blackTimeInSecond = interval;
                    var vmEngine = new DetailsViewModel(MainUtils.VM.MainBord, whiteTimeInSecond, blackTimeInSecond, MainUtils.FromGridIndex, null, fromIndex, toIndex);
                    //dans le cas de loaded
                    /* vmEngine.HuntingBoardWhiteImageList = MainUtils.HuntingBoardWhiteImageList;
                     vmEngine.HuntingBoardBlackImageList = MainUtils.HuntingBoardBlackImageList;
                     vmEngine.MovingList = MainUtils.MovingList;*/

                    vmEngine.DateTimeNow = dateTimeNow;
                    vmEngine.FromGridIndex = MainUtils.FromGridIndex;
                    // return View(MainUtils.VM);


                    if (MainUtils.CurrentTurnColor == "B")
                        MainUtils.CurrentTurnColor = "W";
                    else
                        MainUtils.CurrentTurnColor = "B";
                    vmEngine.CurrentTurn = MainUtils.CurrentTurnColor;
                    vmEngine.ComputerColor = MainUtils.CPUColor;

                    vmEngine.InitialDuration = MainUtils.InitialDuration;
                    MainUtils.MovingList = vmEngine.MovingList;
                    if (MainUtils.CPUColor == "B")
                        vmEngine.RevertWrapperClass = "revertWrapper";

                    if (vmEngine.MainBord.WhiteScore < vmEngine.MainBord.BlackScore)
                        vmEngine.BlackScore = $"+{(vmEngine.MainBord.BlackScore - vmEngine.MainBord.WhiteScore).ToString()}";
                    else if (vmEngine.MainBord.BlackScore < vmEngine.MainBord.WhiteScore)
                        vmEngine.WhiteScore = $"+{(vmEngine.MainBord.WhiteScore - vmEngine.MainBord.BlackScore).ToString()}";
                    else
                        vmEngine.BlackScore = vmEngine.WhiteScore = "";

                    MainUtils.CaseList = vmEngine.MainBord.GetCases().ToList();
                    // System.GC.Collect();
                    //                GC.Collect();

                    //on remet MainUtils.CpuCount à 0 per permetre le reflection du CPU au prochain tour
                    MainUtils.CpuCount = 0;
                    return PartialView("Details", vmEngine);
                }

                }
            catch (Exception)
            {

                throw;
            }
            finally
            {
              ////  GC.Collect();
            }
           
            
            // }
            // return PartialView("Details");
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return Content("file not selected");

                var fileFullPath = Path.Combine(
                            Directory.GetCurrentDirectory(), "wwwroot", "UploadedFiles",
                            file.FileName);

                if (System.IO.File.Exists(fileFullPath))
                    System.IO.File.Delete(fileFullPath);

                using (var stream = new FileStream(fileFullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                //LECTURE DU FICHIER ZIP
                //decopression
                var destinationDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedFiles",file.FileName.Replace(".Chess.zip", ""));

                var exists = System.IO.Directory.Exists(destinationDirectory);

                if (exists)
                {

                    var di = new DirectoryInfo(destinationDirectory);
                    FileInfo[] files = di.GetFiles();
                    foreach (FileInfo fil in files)
                    {
                        fil.Delete();
                    }
                    System.IO.Directory.Delete(destinationDirectory);
                }


                System.IO.Directory.CreateDirectory(destinationDirectory);


                ZipFile.ExtractToDirectory(fileFullPath, destinationDirectory);

                //lecture des positions
                var destinationExtractedPath = Path.Combine(fileFullPath, destinationDirectory);
                var whiteFileLocation = $"{destinationExtractedPath}/WHITEList.txt";
                var blackFileLocation = $"{destinationExtractedPath}/BlackList.txt";
                var huntingBoardWhiteImageListLoaction = $"{destinationExtractedPath}/huntingBoardWhiteImageList.txt";
                var huntingBoardBlackImageListLoaction = $"{destinationExtractedPath}/huntingBoardBlackImageList.txt";
                var historyLoaction = $"{destinationExtractedPath}/History.txt";


                var pawnList = new List<Tools.Pawn>();

                var readText = System.IO.File.ReadAllText(whiteFileLocation);
                using (StringReader sr = new StringReader(readText))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        //Debug.WriteLine(line);

                        var datas = line.Split(';');

                        var newPawn = new Tools.Pawn(datas[0], datas[1],  datas[2]);
                        //;{pawn.IsFirstMove};{pawn.IsFirstMoveKing};{pawn.IsLeftRookFirstMove};{pawn.IsRightRookFirstMove}
                        newPawn.IsFirstMove = bool.Parse(datas[3]);
                        newPawn.IsFirstMoveKing = bool.Parse(datas[4]);
                        newPawn.IsLeftRookFirstMove = bool.Parse(datas[5]);
                        newPawn.IsRightRookFirstMove = bool.Parse(datas[6]);
                        pawnList.Add(newPawn);

                    }
                }
                readText = System.IO.File.ReadAllText(blackFileLocation);
                using (StringReader sr = new StringReader(readText))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        //Debug.WriteLine(line);

                        var datas = line.Split(';');

                        var newPawn = new Tools.Pawn(datas[0], datas[1], datas[2]);
                        //;{pawn.IsFirstMove};{pawn.IsFirstMoveKing};{pawn.IsLeftRookFirstMove};{pawn.IsRightRookFirstMove}
                        newPawn.IsFirstMove = bool.Parse(datas[3]);
                        newPawn.IsFirstMoveKing = bool.Parse(datas[4]);
                        newPawn.IsLeftRookFirstMove = bool.Parse(datas[5]);
                        newPawn.IsRightRookFirstMove = bool.Parse(datas[6]);
                        pawnList.Add(newPawn);

                    }
                }

                var huntingBoardWhiteImageList = new List<string>();
                var huntingBoardBlackImageList = new List<string>();
                var historyList = new List<string>();

                readText = System.IO.File.ReadAllText(huntingBoardWhiteImageListLoaction);
                using (StringReader sr = new StringReader(readText))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        huntingBoardWhiteImageList.Add(line);
                    }
                }
                readText = System.IO.File.ReadAllText(huntingBoardBlackImageListLoaction);
                using (StringReader sr = new StringReader(readText))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        huntingBoardBlackImageList.Add(line);
                    }
                }
                readText = System.IO.File.ReadAllText(historyLoaction);
                using (StringReader sr = new StringReader(readText))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        historyList.Add(line);
                    }
                }


                var mainBord = Chess2Utils.GenerateBoardFormPawnList(pawnList);
                mainBord.HuntingBoardWhiteImageList = huntingBoardWhiteImageList;
                mainBord.HuntingBoardBlackImageList = huntingBoardBlackImageList;
                mainBord.MovingList = historyList;

                MainUtils.VM = new MainPageViewModel(mainBord);
                MainUtils.VM.IsFormLoander = true;
                MainUtils.VM.HuntingBoardWhiteImageList = huntingBoardWhiteImageList;
                MainUtils.VM.HuntingBoardBlackImageList = huntingBoardBlackImageList;
                MainUtils.VM.MovingList = historyList;
                MainUtils.VM.MainBord.CalculeScores();
                //pour les scores
                if (MainUtils.VM.MainBord.WhiteScore < MainUtils.VM.MainBord.BlackScore)
                    MainUtils.VM.BlackScore = $"+{(MainUtils.VM.MainBord.BlackScore - MainUtils.VM.MainBord.WhiteScore).ToString()}";
                else if (MainUtils.VM.MainBord.BlackScore < MainUtils.VM.MainBord.WhiteScore)
                    MainUtils.VM.WhiteScore = $"+{(MainUtils.VM.MainBord.WhiteScore - MainUtils.VM.MainBord.BlackScore).ToString()}";
                else
                    MainUtils.VM.BlackScore = MainUtils.VM.WhiteScore = "";
                ViewBag.Message = "File Uploaded Successfully!!";
                return View("Index", MainUtils.VM);
            }
            catch (Exception ex)
            {
                ViewBag.Message = "File upload failed!!";
                Debug.WriteLine(ex.ToString());
                return View();
                throw;
            }




            

         //   return RedirectToAction("Files");
        }

      

        [HttpPost]
        public ActionResult Details(int objId, int whiteTimeInSecond, int blackTimeInSecond, string CPUColor, string selectedDurationType, int selectedLevel)
        {
          ////  GC.Collect();
            //var t_ = selectionLevel;
            if (selectedLevel != -1)
                MainUtils.DeepLevel = selectedLevel;
            MainUtils.InitialDuration = 30 * 60;
            if (selectedDurationType != null)
            {
                if (selectedDurationType == "15mn")
                    MainUtils.InitialDuration = 15 * 60;
                if (selectedDurationType == "1h")
                    MainUtils.InitialDuration = 60 * 60;
            }

            var dateTimeNow = DateTime.Now;
            var posiblesMoveListSelectedPawn = new List<int>();

            var oldLocationIndex = -1;
            var isMove = false;

            if (objId == -1)//Initialisation
            {
                dateTimeNow = DateTime.Now;


                var initialVm = new DetailsViewModel(MainUtils.VM.MainBord, whiteTimeInSecond, blackTimeInSecond);
                //dans le cas de loaded
                /* initialVm.HuntingBoardWhiteImageList = MainUtils.VM.HuntingBoardWhiteImageList;
                 initialVm.HuntingBoardBlackImageList = MainUtils.VM.HuntingBoardBlackImageList;
                 initialVm.MovingList = MainUtils.VM.MovingList;*/
                MainUtils.HuntingBoardWhiteImageList = MainUtils.VM.HuntingBoardWhiteImageList;
                MainUtils.HuntingBoardBlackImageList = MainUtils.VM.HuntingBoardBlackImageList;

                initialVm.DateTimeNow = dateTimeNow;
                // return View(MainUtils.VM);
                MainUtils.CPUColor = CPUColor;
                MainUtils.CurrentTurnColor = "W";
                initialVm.CurrentTurn = MainUtils.CurrentTurnColor;
                initialVm.ComputerColor = MainUtils.CPUColor;
                initialVm.InitialDuration = MainUtils.InitialDuration;
                if (MainUtils.CPUColor == "B")
                    initialVm.RevertWrapperClass = "revertWrapper";
                if (initialVm.MainBord.WhiteScore < initialVm.MainBord.BlackScore)
                    initialVm.BlackScore = $"+{(initialVm.MainBord.BlackScore - initialVm.MainBord.WhiteScore).ToString()}";
                else if (initialVm.MainBord.BlackScore < initialVm.MainBord.WhiteScore)
                    initialVm.WhiteScore = $"+{(initialVm.MainBord.WhiteScore - initialVm.MainBord.BlackScore).ToString()}";
                else
                    initialVm.BlackScore = initialVm.WhiteScore = "";
   
                return PartialView("Details", initialVm);
            }
            // NorthwindEntities entities = new NorthwindEntities();

            //SELECTION
            else if (MainUtils.FromGridIndex == -1)
            {
                MainUtils.FromGridIndex = objId;
                posiblesMoveListSelectedPawn = MainUtils.VM.MainBord.GetPossibleMoves(MainUtils.FromGridIndex, 0).Select(x => x.Index).ToList();


            }
            //DEPLACEMENT
            else //if (MainUtils.CurrentTurnColor != MainUtils.CPUColor)
            {
                
                
                   
                isMove = true;
                MainUtils.ToGridIndex = objId;

                var selectedPawn = MainUtils.VM.GetPawn(MainUtils.FromGridIndex);

                posiblesMoveListSelectedPawn = MainUtils.VM.MainBord.GetPossibleMoves(MainUtils.FromGridIndex, 0).Select(x => x.Index).ToList();
                if (!posiblesMoveListSelectedPawn.Contains(MainUtils.ToGridIndex) || (selectedPawn.PawnColor != MainUtils.CurrentTurnColor))
                {
                    //si mouvement impossible, on ne fait rien
                    MainUtils.FromGridIndex = -1;
                    //return null;
                    dateTimeNow = DateTime.Now;
                    /*var mainBord = new Board();
                    mainBord.Init();*/
                    var vmOld = new DetailsViewModel(MainUtils.VM.MainBord, whiteTimeInSecond, blackTimeInSecond);
                    vmOld.DateTimeNow = dateTimeNow;
                    vmOld.CurrentTurn = MainUtils.CurrentTurnColor;
                    vmOld.ComputerColor = MainUtils.CPUColor;
                    vmOld.InitialDuration = MainUtils.InitialDuration;
                    vmOld.WhiteTimeInSecond = whiteTimeInSecond;

                    //dans le cas de loaded
                    /* vmOld.HuntingBoardWhiteImageList = MainUtils.VM.HuntingBoardWhiteImageList;
                     vmOld.HuntingBoardBlackImageList = MainUtils.VM.HuntingBoardBlackImageList;
                     vmOld.MovingList = MainUtils.VM.MovingList;*/

                    MainUtils.MovingList = vmOld.MovingList;
                    if (MainUtils.CPUColor == "B")
                        vmOld.RevertWrapperClass = "revertWrapper";
                    if (vmOld.MainBord.WhiteScore < vmOld.MainBord.BlackScore)
                        vmOld.BlackScore = $"+{(vmOld.MainBord.BlackScore - vmOld.MainBord.WhiteScore).ToString()}";
                    else if (vmOld.MainBord.BlackScore < vmOld.MainBord.WhiteScore)
                        vmOld.WhiteScore = $"+{(vmOld.MainBord.WhiteScore - vmOld.MainBord.BlackScore).ToString()}";
                    else
                        vmOld.BlackScore = vmOld.WhiteScore = "";
                    // return View(MainUtils.VM);
                    MainUtils.CaseList = vmOld.MainBord.GetCases().ToList();

                    return PartialView("Details", vmOld);

                }
                MainUtils.VM.MainBord.Move(MainUtils.FromGridIndex, MainUtils.ToGridIndex, 0);
                MainUtils.TurnNumber++;
                MainUtils.VM.Refresh(MainUtils.VM.MainBord);
                oldLocationIndex = MainUtils.FromGridIndex;
                MainUtils.FromGridIndex = -1;
                if (MainUtils.CurrentTurnColor == "B")
                    MainUtils.CurrentTurnColor = "W";
                else
                    MainUtils.CurrentTurnColor = "B";




            }

            var movedOldLocationIndex = -1;
            var movedNewLocation = -1;
            if (isMove)
            {
                movedOldLocationIndex = oldLocationIndex;
                movedNewLocation = MainUtils.ToGridIndex;
            }
            var vm = new DetailsViewModel(MainUtils.VM.MainBord, whiteTimeInSecond, blackTimeInSecond, MainUtils.FromGridIndex, posiblesMoveListSelectedPawn, movedOldLocationIndex, movedNewLocation);


            vm.DateTimeNow = dateTimeNow;
            vm.FromGridIndex = MainUtils.FromGridIndex;
            vm.CurrentTurn = MainUtils.CurrentTurnColor;
            vm.ComputerColor = MainUtils.CPUColor;
            vm.InitialDuration = MainUtils.InitialDuration;
            MainUtils.MovingList = vm.MovingList;
            if (MainUtils.CPUColor == "B")
                vm.RevertWrapperClass = "revertWrapper";

            if (vm.MainBord.WhiteScore < vm.MainBord.BlackScore)
                vm.BlackScore = $"+{(vm.MainBord.BlackScore - vm.MainBord.WhiteScore).ToString()}";
            else if (vm.MainBord.BlackScore < vm.MainBord.WhiteScore)
                vm.WhiteScore = $"+{(vm.MainBord.WhiteScore - vm.MainBord.BlackScore).ToString()}";
            else
                vm.BlackScore = vm.WhiteScore = "";
            // return View(MainUtils.VM);

            MainUtils.CaseList = vm.MainBord.GetCases().ToList();
            //            GC.Collect();
            return PartialView("Details", vm);
        }


        public FileResult SavePrintScreen(string image)
        {
            var dateTimeString = DateTime.Now.ToString("HH-mm-ss dd-MM-yyyy");
            //TODO il faut ajouter une boite de dialoge demandant le nom du docier
            var dirName = dateTimeString;
            //var dirLocalPath = Server.MapPath($"~/ToDownloadedFiles/{dirName}");
            var dirLocalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ToDownloadedFiles", $"{dirName}");

          //  var dirLocalPath = ($"~/ToDownloadedFiles/{dirName}");



            if (Directory.Exists(dirLocalPath))
                System.IO.Directory.Delete(dirLocalPath);
            System.IO.Directory.CreateDirectory(dirLocalPath);



            // var t_board = MainUtils.VM.MainBord;
            var caseList = MainUtils.CaseList;
            if (caseList == null)
                return null;
            var caseListStr = String.Join("\n", caseList);
            //  var dirPath = AppDomain.CurrentDomain.BaseDirectory + dirName;// $"~/{dateTimeString}";
            if (Directory.Exists(dirLocalPath))
            {

                //Image
                image = image.Replace("data:image/octet-stream;base64,", "");
                //var bytes = Convert.FromBase64String(image);
                var imageFileName = $"IMG.png";
                //using (var imageFile = new FileStream(filePath, FileMode.Create))
                //{
                //  imageFile.Write(bytes, 0, bytes.Length);
                //  imageFile.Flush();
                //}

                var imageDestinationPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ToDownloadedFiles", $"{dirName}{imageFileName}");

               // var imageDestinationPath = Path.Combine("~", "//", );
                if (System.IO.File.Exists(imageDestinationPath))
                {
                    System.IO.File.Delete(imageDestinationPath);
                }

                using (FileStream fs = new FileStream(imageDestinationPath, FileMode.Create))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        byte[] bytes = Convert.FromBase64String(image);
                        bw.Write(bytes, 0, bytes.Length);
                        bw.Close();
                    }
                }


                //Fichier
                var pawnStringList = Chess2Utils.GeneratePawnStringListFromCaseList(caseList);
                var pawnStringWhite = String.Join("\n", pawnStringList.Where(x => x.Contains("White")).ToList());
                var pawnStringBlack = String.Join("\n", pawnStringList.Where(x => x.Contains("Black")).ToList());
                System.IO.File.WriteAllText($"{dirLocalPath}/caseList.txt", caseListStr);
                System.IO.File.WriteAllText($"{dirLocalPath}/WHITEList.txt", pawnStringWhite);
                System.IO.File.WriteAllText($"{dirLocalPath}/BLACKList.txt", pawnStringBlack);

                //historique
                var movingListStr = String.Join("\n", MainUtils.MovingList); //MainUtils.MovingList.Join( ("\\n");
                var historyFileName = $"{dateTimeString}History.txt";
                System.IO.File.WriteAllText($"{dirLocalPath}/History.txt", movingListStr);

                //HuntingBoardBlackImageList et HuntingBoardWhiteImageList

                var huntingBoardWhiteImageListString = "";
                var huntingBoardBlackImageListString = "";
                if (MainUtils.HuntingBoardWhiteImageList != null)
                    huntingBoardWhiteImageListString = String.Join("\n", MainUtils.HuntingBoardWhiteImageList);
                if (MainUtils.HuntingBoardBlackImageList != null)
                    huntingBoardBlackImageListString = String.Join("\n", MainUtils.HuntingBoardBlackImageList);

                System.IO.File.WriteAllText($"{dirLocalPath}/huntingBoardWhiteImageList.txt", huntingBoardWhiteImageListString);
                System.IO.File.WriteAllText($"{dirLocalPath}/huntingBoardBlackImageList.txt", huntingBoardBlackImageListString);


            }



            //crée le fichierzip
            var zipDirPath = Path.Combine(dirLocalPath);
            MainUtils.ZipFilePath = $"{zipDirPath}.Chess.zip";
            MainUtils.ZipFileName = $"{dirName}.Chess.zip";

            ZipFile.CreateFromDirectory(zipDirPath, MainUtils.ZipFilePath);



            byte[] fileBytes = System.IO.File.ReadAllBytes(MainUtils.ZipFilePath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, MainUtils.ZipFilePath);
        }


        //retourne le fichier zip
        public FileResult SaveBoard()
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(MainUtils.ZipFilePath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, MainUtils.ZipFileName);

        }

        public FileResult SaveHistory()
        {
            var movingListStr = String.Join("\n", MainUtils.MovingList); //MainUtils.MovingList.Join( ("\\n");
            var dateTimeString = DateTime.Now.ToString("HH-mm-ss dd-MM-yyyy");
            // _partHistoryDestinationFileFullPath = Path.Combine(_destinationHistoryFolderPath, );
            //var historyFilePath = $"~/{dateTimeString}History.txt";

            var historyFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Histories", $"{dateTimeString}History.txt");

            var historyFileName = $"{dateTimeString}History.txt";
            System.IO.File.WriteAllText(historyFilePath, movingListStr);



            byte[] fileBytes = System.IO.File.ReadAllBytes(historyFilePath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, historyFileName);



            //  return null;
        }





        [HttpGet]
        public ActionResult Index()
        {
            //var t_id = id;


            var mainBord = new Board();
            mainBord.Init();
            MainUtils.VM = new MainPageViewModel(mainBord);
            return View(MainUtils.VM);




        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}