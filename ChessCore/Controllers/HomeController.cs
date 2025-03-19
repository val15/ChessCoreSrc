using ChessCore.Models;
using ChessCore.Tools;
using ChessCore.Tools.ChessEngine;
using ChessCore.Tools.ChessEngine.Engine;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;
using System.Diagnostics;
using System.IO.Compression;


namespace ChessCore.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        private int _blackCPULevel;
        private int _whiteCPULevel;
        private int _CPULevel;
        private bool _isCHECKMATE = false;

        //private IChessEngine _chessEngine;
        //private 


        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            // _chessEngine = new ChessEngine3();
            _logger = logger;
            _configuration = configuration;
            //_CPULevel = _configuration.GetValue<int>("CPUSettings:CPULevel");
            //_blackCPULevel = _configuration.GetValue<int>("CPUSettings:BlackCPULevel");
            //_whiteCPULevel = _configuration.GetValue<int>("CPUSettings:WhiteCPULevel");
            _CPULevel = Utils.DeepLevel;

        }

        [HttpGet]
        public ActionResult Index()
        {

            //INIT ALL
            MainUtils.VM = null;
            var mainBord = new Board();
            mainBord.Init();
            MainUtils.VM = new MainPageViewModel(mainBord);
            InitMainPageViewModel(MainUtils.VM);


            return View(MainUtils.VM);
        }

        public void InitMainPageViewModel(MainPageViewModel mainViewModel) 
        {
            //MainUtils.VM.Engines.Add(new ChessEngine1());
            //MainUtils.VM.Engines.Add(new ChessEngine2());
            MainUtils.VM.Engines.Add(new ChessEngine3());
            MainUtils.VM.Engines.Add(new Tools.ChessEngine.Engine.SS.ChessEngine3SS());
            MainUtils.VM.Engines.Add(new ChessEngineStockfish());
            //MainUtils.VM.Engines.Add(new ChessEngineLLM());

            MainUtils.VM.SelectedEngine = MainUtils.VM.Engines[1]; //.Last();
            MainUtils.VM.SelectedWhiteEngine = MainUtils.VM.Engines[1]; //.Last();
            MainUtils.VM.SelectedBlackEngine = MainUtils.VM.Engines[2]; //.Last();
            MainUtils.VM.SelectedLevel = 6;
            MainUtils.VM.SelectedReflectionTimeInMinute = 2;
            MainUtils.VM.SelectedWhiteLevel = 6;
            MainUtils.VM.SelectedBlackLevel = 6;
        }




        [HttpPost]
        //MOVEMENT DU CPU
        public ActionResult DetailsTimer()
        {
            try
            {
                var whiteKing = MainUtils.VM.Cases.FirstOrDefault(x => x == "K|W");
                var blackKing = MainUtils.VM.Cases.FirstOrDefault(x => x == "K|B");
                if (whiteKing == null)
                {
                    Utils.WritelineAsync("Black Wins");
                    return View("Result", "Black");
                }
                else if (blackKing == null)
                {
                    Utils.WritelineAsync("White Wins");
                    return View("Result", "White");
                }
                //if (_isCHECKMATE)
                //    return null;
                MainUtils.CpuCount++;
                //   var t_ = "cpu";

                //si CpuCount est supperier à 1, on ne prend pas en compt car le cpu est
                // deja en cour de refelextion
                if (MainUtils.CpuCount > 1)
                    return null;

                //pour le timer,
                //il faut prendre l'intervale en seconde et l'ajouter à 
                //computer timer


                var currentBoard = new BoardCE(MainUtils.VM.MainBord.GetCases());
                currentBoard.Print();
                var dsf = currentBoard.ToString();
                if (currentBoard.IsKingInCheck(MainUtils.CPUColor) || currentBoard.IsKingInCheck(MainUtils.CPUColor))
                {
                    Utils.WritelineAsync("CHECKMATE");
                    _isCHECKMATE = true;
                    var winVM = new DetailsViewModel();
                    winVM.SelectedLevel = Utils.DeepLevel;
                    winVM.SelectedReflectionTimeInMinute = Utils.SelectedReflectionTimeInMinute;
                    if (MainUtils.CPUColor == "W")
                        winVM.StringWinnerColor = "Balck WIN";
                    else
                        winVM.StringWinnerColor = "White WIN";
                    winVM.IsCHECKMATE = true;
                    return PartialView("Details", winVM);

                }


                Utils.DeepLevel = _CPULevel;

                var bestNode = Utils.RunEngine(MainUtils.VM.SelectedEngine, MainUtils.CPUColor, currentBoard, Utils.DeepLevel,Utils.SelectedReflectionTimeInMinute);


                var fromIndex = CoordTools.GetIndexFromLocation(bestNode.Location);//int
                var toIndex = CoordTools.GetIndexFromLocation(bestNode.BestChildPosition);

                //determination si attaque pour remplir le cimetiere
                //  var destinationCase = MainUtils.VM.MainBord.GetCases()[bestNodeChess2.ToIndex];
                //le CPU a perdu si bestNodeChess2.FromIndex == bestNodeChess2.ToIndex
                if (fromIndex == toIndex)
                    return View("Losing");


                MainUtils.VM.MainBord.Move(fromIndex, toIndex);
                MainUtils.TurnNumber++;
                MainUtils.VM.Refresh(MainUtils.VM.MainBord);
                MainUtils.FromGridIndex = -1;


                var vmEngine = new DetailsViewModel(MainUtils.VM.MainBord, MainUtils.FromGridIndex, null, fromIndex, toIndex);
                vmEngine.SelectedLevel = Utils.DeepLevel;
                vmEngine.SelectedReflectionTimeInMinute = Utils.SelectedReflectionTimeInMinute;
                //dans le cas de loaded
                /* vmEngine.HuntingBoardWhiteImageList = MainUtils.HuntingBoardWhiteImageList;
                 vmEngine.HuntingBoardBlackImageList = MainUtils.HuntingBoardBlackImageList;
                 vmEngine.MovingList = MainUtils.MovingList;*/

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
                    vmEngine.BlackScore = (vmEngine.MainBord.BlackScore - vmEngine.MainBord.WhiteScore);
                else if (vmEngine.MainBord.BlackScore < vmEngine.MainBord.WhiteScore)
                    vmEngine.WhiteScore = (vmEngine.MainBord.WhiteScore - vmEngine.MainBord.BlackScore);
                else
                    vmEngine.BlackScore = vmEngine.WhiteScore = 0;

                MainUtils.CaseList = vmEngine.MainBord.GetCases().ToList();


                // System.GC.Collect();
                //                GC.Collect();

                //on remet MainUtils.CpuCount à 0 per permetre le reflection du CPU au prochain tour
                MainUtils.CpuCount = 0;
                if (MainUtils.IsFullCPU)
                    vmEngine.IsFullCPU = 1;
                else
                    vmEngine.IsFullCPU = 0;

                //couleurs des levels
                if (MainUtils.CPUColor == "B")
                {
                    vmEngine.StringBlackCPULevel = $"L {_CPULevel}";
                    vmEngine.StringWhiteCPULevel = $"L {0}";
                }
                else
                {
                    vmEngine.StringWhiteCPULevel = $"L {_CPULevel}";
                    vmEngine.StringBlackCPULevel = $"L {0}";
                }
                vmEngine.IsMove = 1;
                return PartialView("Details", vmEngine);


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
        //MOVEMENT CPU vs CPU
        public ActionResult DetailsTimerFULLCPU()
        {
            try
            {
                //WINER 
                
                var whiteKing = MainUtils.VM.Cases.FirstOrDefault(x => x == "K|W");
                var blackKing = MainUtils.VM.Cases.FirstOrDefault(x => x == "K|B");
                if (whiteKing==null)
                {
                    Utils.WritelineAsync("Black Wins");
                    return View("Result", "Black");
                }
                else if (blackKing == null)
                {
                    Utils.WritelineAsync("White Wins");
                    return View("Result", "White");
                }
                // return View("Losing");
                //if (_isCHECKMATE)
                //    return null;


                MainUtils.CpuCount++;
                if (MainUtils.CpuCount > 1)
                    return null;
                var degingDateTimeNow = DateTime.Now;
                //pour le timer,
                //il faut prendre l'intervale en seconde et l'ajouter à 
                //computer timer
                // var startRefelectionTime = DateTime.Now;

                //Méthode nonothread
                // var engine = new Engine(MainUtils.DeepLevel, MainUtils.CPUColor, false, null);
                //  var bestNodeChess2 = engine.Search(MainUtils.VM.MainBord, MainUtils.TurnNumber.ToString(), -1, -1);
                //méthode multithreading



                var fromIndex = -1;
                var toIndex = -1;


                Utils.WritelineAsync($"Current turn = {MainUtils.CurrentTurnColor} => normlaEngine");

                NodeCE bestNode;
                var depthLevel = 3;
                if (MainUtils.CPUColor == "W")
                {
                    depthLevel = MainUtils.FullCPUWhiteLevel;
                    bestNode = Utils.RunEngine(MainUtils.VM.SelectedWhiteEngine, MainUtils.CurrentTurnColor, new BoardCE(MainUtils.VM.MainBord.GetCases()), depthLevel, Utils.SelectedReflectionTimeInMinute);

                }
                else
                {
                    depthLevel = MainUtils.FullCPUBlackLevel;
                    bestNode = Utils.RunEngine(MainUtils.VM.SelectedBlackEngine, MainUtils.CurrentTurnColor, new BoardCE(MainUtils.VM.MainBord.GetCases()), depthLevel, Utils.SelectedReflectionTimeInMinute);


                }









                if (bestNode == null)//ECHES ET MATE
                {
                    Utils.WritelineAsync("CHECKMATE");
                    _isCHECKMATE = true;
                    var winVM = new DetailsViewModel();
                    winVM.SelectedLevel = Utils.DeepLevel;
                    winVM.SelectedReflectionTimeInMinute = Utils.SelectedReflectionTimeInMinute;
                    //if (MainUtils.CPUColor == "W")
                    //    winVM.StringWinnerColor = "Balck WIN";
                    //else
                    //    winVM.StringWinnerColor = "White WIN";
                    //winVM.IsCHECKMATE = true;
                    //return PartialView("Details", winVM);

                    if (MainUtils.CPUColor == "B")
                    {
                        Utils.WritelineAsync("Black Wins");
                        return View("Result", "Black");
                    }
                    else
                    {
                        Utils.WritelineAsync("White Wins");
                        return View("Result", "White");
                    }


                }
                fromIndex = CoordTools.GetIndexFromLocation(bestNode.Location);//int
                toIndex = CoordTools.GetIndexFromLocation(bestNode.BestChildPosition);





                //determination si attaque pour remplir le cimetiere
                //  var destinationCase = MainUtils.VM.MainBord.GetCases()[bestNodeChess2.ToIndex];
                //le CPU a perdu si bestNodeChess2.FromIndex == bestNodeChess2.ToIndex
                if (fromIndex == toIndex)
                    return View("Losing");


                MainUtils.VM.MainBord.Move(fromIndex, toIndex);
                MainUtils.TurnNumber++;
                MainUtils.VM.Refresh(MainUtils.VM.MainBord);
                MainUtils.FromGridIndex = -1;

                var vmEngine = new DetailsViewModel(MainUtils.VM.MainBord, MainUtils.FromGridIndex, null, fromIndex, toIndex);
                vmEngine.SelectedLevel = Utils.DeepLevel;
                vmEngine.SelectedReflectionTimeInMinute = Utils.SelectedReflectionTimeInMinute;
                //dans le cas de loaded
                /* vmEngine.HuntingBoardWhiteImageList = MainUtils.HuntingBoardWhiteImageList;
                 vmEngine.HuntingBoardBlackImageList = MainUtils.HuntingBoardBlackImageList;
                 vmEngine.MovingList = MainUtils.MovingList;*/

                vmEngine.DateTimeNow = degingDateTimeNow;

                vmEngine.FromGridIndex = MainUtils.FromGridIndex;
                // return View(MainUtils.VM);


                if (MainUtils.CurrentTurnColor == "B")
                {
                    MainUtils.CurrentTurnColor = "W";
                }
                else
                {
                    MainUtils.CurrentTurnColor = "B";
                }

                vmEngine.CurrentTurn = MainUtils.CurrentTurnColor;
                //Mode ful CPU
                MainUtils.CPUColor = MainUtils.CurrentTurnColor;
                vmEngine.ComputerColor = MainUtils.CPUColor;

                vmEngine.InitialDuration = MainUtils.InitialDuration;
                MainUtils.MovingList = vmEngine.MovingList;
                //  if (MainUtils.CPUColor == "B")
                //       vmEngine.RevertWrapperClass = "revertWrapper";

                if (vmEngine.MainBord.WhiteScore < vmEngine.MainBord.BlackScore)
                    vmEngine.BlackScore = (vmEngine.MainBord.BlackScore - vmEngine.MainBord.WhiteScore);
                else if (vmEngine.MainBord.BlackScore < vmEngine.MainBord.WhiteScore)
                    vmEngine.WhiteScore = (vmEngine.MainBord.WhiteScore - vmEngine.MainBord.BlackScore);
                else
                    vmEngine.BlackScore = vmEngine.WhiteScore = 0;

                MainUtils.CaseList = vmEngine.MainBord.GetCases().ToList();
                // System.GC.Collect();
                //                GC.Collect();

                //on remet MainUtils.CpuCount à 0 per permetre le reflection du CPU au prochain tour
                MainUtils.CpuCount = 0;
                if (MainUtils.IsFullCPU)
                    vmEngine.IsFullCPU = 1;
                else
                    vmEngine.IsFullCPU = 0;


                vmEngine.StringBlackCPULevel = $"L {MainUtils.FullCPUBlackLevel}";
                vmEngine.StringWhiteCPULevel = $"L {MainUtils.FullCPUWhiteLevel}";
                return PartialView("Details", vmEngine);


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
                MainUtils.MovingListIndex = -1;
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
                var destinationDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedFiles", file.FileName.Replace(".Chess.zip", ""));

                //var exists = System.IO.Directory.Exists(destinationDirectory);
                if (Directory.Exists(destinationDirectory)) Directory.Delete(destinationDirectory, true);



                System.IO.Directory.CreateDirectory(destinationDirectory);


                ZipFile.ExtractToDirectory(fileFullPath, destinationDirectory);

                //lecture des positions
                var destinationExtractedPath = Path.Combine(fileFullPath, destinationDirectory);
                if (!Directory.Exists(destinationExtractedPath))
                    Directory.CreateDirectory(destinationExtractedPath);
                var whiteFileLocation = $"{destinationExtractedPath}/WHITEList.txt";
                var blackFileLocation = $"{destinationExtractedPath}/BLACKList.txt";
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

                readText = System.IO.File.ReadAllText(blackFileLocation);
                using (StringReader sr = new StringReader(readText))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {


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
                        //historyList.Add(Utils.LineToSymbol(line));
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
                MainUtils.VM.MainBord.CalculeScores(Utils.ComputerColor);
                InitMainPageViewModel(MainUtils.VM);
                //pour les scores
                if (MainUtils.VM.MainBord.WhiteScore < MainUtils.VM.MainBord.BlackScore)
                    MainUtils.VM.BlackScore = (MainUtils.VM.MainBord.BlackScore - MainUtils.VM.MainBord.WhiteScore);
                else if (MainUtils.VM.MainBord.BlackScore < MainUtils.VM.MainBord.WhiteScore)
                    MainUtils.VM.WhiteScore = (MainUtils.VM.MainBord.WhiteScore - MainUtils.VM.MainBord.BlackScore);
                else
                    MainUtils.VM.BlackScore = MainUtils.VM.WhiteScore = 0;
                ViewBag.Message = "File Uploaded Successfully!!";
                return View("Index", MainUtils.VM);
            }
            catch (Exception ex)
            {
                ViewBag.Message = "File upload failed!!";
                Utils.WritelineAsync(ex.ToString());
                return View();
                throw;
            }






            //   return RedirectToAction("Files");
        }

        /// <summary>
        /// tsiry;02-07-2022
        /// fonction Preview, à partie de MainUtils.MovingList,
        /// on revien en arrier
        /// </summary>
        public IActionResult Preview()
        {
            try
            {
                if (MainUtils.MovingListIndex == 0)//si on est au debut, on ne pouge plus
                {
                    MainUtils.MovingListIndex = 1;
                }
                //bestNodeList.Where().Select(c => { c.Weight = node.Weight; return c; }).ToList();
                //on enleve le courseur
                for (var i = 0; i < MainUtils.VM.MainBord.MovingList.Count(); i++)
                {
                    if (MainUtils.VM.MainBord.MovingList[i].Contains(Utils.NavigationStoryCursor))
                    {
                        MainUtils.VM.MainBord.MovingList[i] = MainUtils.VM.MainBord.MovingList[i].Replace(Utils.NavigationStoryCursor, "");

                    }
                }
                var oldMovingList = MainUtils.VM.MainBord.MovingList;
                var lastMoveStr = "";
                if (MainUtils.MovingListIndex == -1)
                    MainUtils.MovingListIndex = MainUtils.VM.MainBord.MovingList.Count();

                MainUtils.MovingListIndex--;
                lastMoveStr = MainUtils.VM.MainBord.MovingList[MainUtils.MovingListIndex];

                var data = lastMoveStr.Split("→");
                var fromSrt = data[0];
                var toSrt = data[1];


                var copyBord = new Board(MainUtils.VM.MainBord);
                copyBord.NavigationMove(toSrt, fromSrt);
                //copyBord.

                //  mainBord.HuntingBoardWhiteImageList = huntingBoardWhiteImageList;
                //  mainBord.HuntingBoardBlackImageList = huntingBoardBlackImageList;
                //   mainBord.MovingList = historyList;

                return SetBoardAndHystoryOfIndexPage(copyBord, oldMovingList);

            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// tsiry;02-07-2022
        /// fonction Next, à partie de MainUtils.MovingList,
        /// on revien en arrier
        /// </summary>
        public IActionResult Next()
        {
            try
            {
                if (MainUtils.MovingListIndex == MainUtils.VM.MainBord.MovingList.Count())//si on està la fin
                {
                    MainUtils.MovingListIndex = MainUtils.VM.MainBord.MovingList.Count - 1;
                }
                //bestNodeList.Where().Select(c => { c.Weight = node.Weight; return c; }).ToList();
                //on enleve le courseur
                for (var i = 0; i < MainUtils.VM.MainBord.MovingList.Count(); i++)
                {
                    if (MainUtils.VM.MainBord.MovingList[i].Contains(Utils.NavigationStoryCursor))
                    {
                        MainUtils.VM.MainBord.MovingList[i] = MainUtils.VM.MainBord.MovingList[i].Replace(Utils.NavigationStoryCursor, "");

                    }
                }
                var oldMovingList = MainUtils.VM.MainBord.MovingList;
                var lastMoveStr = "";
                //   if(MainUtils.MovingListIndex == -1 )
                //       MainUtils.MovingListIndex =MainUtils.VM.MainBord.MovingList.Count();

                if (MainUtils.MovingListIndex == -1)
                    MainUtils.MovingListIndex = MainUtils.VM.MainBord.MovingList.Count() - 1;
                lastMoveStr = MainUtils.VM.MainBord.MovingList[MainUtils.MovingListIndex];

                var data = lastMoveStr.Split("→");
                var fromSrt = data[0];
                var toSrt = data[1];


                var copyBord = new Board(MainUtils.VM.MainBord);
                copyBord.NavigationMove(fromSrt, toSrt, true);
                //copyBord.

                //  mainBord.HuntingBoardWhiteImageList = huntingBoardWhiteImageList;
                //  mainBord.HuntingBoardBlackImageList = huntingBoardBlackImageList;
                //   mainBord.MovingList = historyList;
                MainUtils.MovingListIndex++;
                return SetBoardAndHystoryOfIndexPage(copyBord, oldMovingList);

            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return null;
            }
        }


        /// <summary>
        /// tsiry;02-07-2022
        //// utiliser dans les fonctrions Preview et next
        /// pour réaficher le boad qui a été modifier avec l'hystorique
        /// </summary>
        IActionResult SetBoardAndHystoryOfIndexPage(Board copyBord, List<string> oldMovingList)
        {
            MainUtils.VM = new MainPageViewModel(copyBord);
            MainUtils.VM.IsFormLoander = true;
            // MainUtils.VM.HuntingBoardWhiteImageList = huntingBoardWhiteImageList;
            //  MainUtils.VM.HuntingBoardBlackImageList = huntingBoardBlackImageList;
            //  MainUtils.VM.MovingList = historyList;
            MainUtils.VM.MainBord.CalculeScores(Utils.ComputerColor);
            //pour les scores
            if (MainUtils.VM.MainBord.WhiteScore < MainUtils.VM.MainBord.BlackScore)
                MainUtils.VM.BlackScore = (MainUtils.VM.MainBord.BlackScore - MainUtils.VM.MainBord.WhiteScore);
            else if (MainUtils.VM.MainBord.BlackScore < MainUtils.VM.MainBord.WhiteScore)
                MainUtils.VM.WhiteScore = (MainUtils.VM.MainBord.WhiteScore - MainUtils.VM.MainBord.BlackScore);
            else
                MainUtils.VM.BlackScore = MainUtils.VM.WhiteScore = 0;
            MainUtils.VM.MainBord.MovingList = oldMovingList;


            MainUtils.VM.MovingList = oldMovingList;
            //ajout d'un curseur
            for (var i = 0; i < MainUtils.VM.MovingList.Count(); i++)
            {
                var line = MainUtils.VM.MovingList[i];
                if (i == MainUtils.MovingListIndex - 1)
                {
                    MainUtils.VM.MovingList[i] = $"{Utils.NavigationStoryCursor}{line}";
                }
            }
            //MainUtils.CaseList?.Clear();
            //MainUtils.CaseList = MainUtils.VM.Cases.ToList();
            return View("Index", MainUtils.VM);
        }



        public FileResult SaveLoadingBoard()
        {



            // var initialVm = new DetailsViewModel(MainUtils.VM.MainBord, whiteTimeInSecond, blackTimeInSecond);

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
            var caseList = MainUtils.VM.MainBord.GetCases().ToList();
            if (caseList == null)
                return null;
            var caseListStr = String.Join("\n", caseList);
            //  var dirPath = AppDomain.CurrentDomain.BaseDirectory + dirName;// $"~/{dateTimeString}";
            if (Directory.Exists(dirLocalPath))
            {

                /*     //Image
                    image = image.Replace("data:image/octet-stream;base64,", "");
                    //var bytes = Convert.FromBase64String(image);
                    var imageFileName = $"IMG.png";
                    //using (var imageFile = new FileStream(filePath, FileMode.Create))
                    //{
                    //  imageFile.Write(bytes, 0, bytes.Length);
                    //  imageFile.Flush();
                    //}

                    var imageDestinationPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ToDownloadedFiles", $"{dirLocalPath}",$"{imageFileName}");

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
    */
                //Fichier
                var pawnStringList = Chess2Utils.GeneratePawnStringListFromCaseList(caseList);
                var pawnStringWhite = String.Join("\n", pawnStringList.Where(x => x.Contains("White")).ToList());
                var pawnStringBlack = String.Join("\n", pawnStringList.Where(x => x.Contains("Black")).ToList());
                System.IO.File.WriteAllText($"{dirLocalPath}/caseList.txt", caseListStr);
                System.IO.File.WriteAllText($"{dirLocalPath}/WHITEList.txt", pawnStringWhite);
                System.IO.File.WriteAllText($"{dirLocalPath}/BLACKList.txt", pawnStringBlack);

                //historique
                var movingListStr = String.Join("\n", MainUtils.VM.MainBord.MovingList); //MainUtils.MovingList.Join( ("\\n");
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


        [HttpPost]
        public ActionResult Details(int objId, string selectedEngine, string selectedWhiteEngine, string selectedBlackEngine, int whiteTimeInSecond, int blackTimeInSecond, string CPUColor, int selectedLevel, bool isFullCPU, int FullCPUWhiteLevel, int FullCPUBlackLevel,int SelectedReflectionTimeInMinute)
        {
            var whiteKing = MainUtils.VM.Cases.FirstOrDefault(x => x == "K|W");
            var blackKing = MainUtils.VM.Cases.FirstOrDefault(x => x == "K|B");
            if (whiteKing == null)
            {
                Utils.WritelineAsync("Black Wins");
                return View("Result", "Black");
            }
            else if (blackKing == null)
            {
                Utils.WritelineAsync("White Wins");
                return View("Result", "White");
            }
            ////  GC.Collect();
            //var t_ = selectionLevel;
            // var t_ = isFullCPU;
            MainUtils.IsFullCPU = isFullCPU;
            MainUtils.FullCPUWhiteLevel = FullCPUWhiteLevel;
            MainUtils.FullCPUBlackLevel = FullCPUBlackLevel;
            MainUtils.FullCPUBlackLevel = FullCPUBlackLevel;

            if (selectedEngine !=null)
                MainUtils.VM.SelectedEngine = MainUtils.VM.Engines.FirstOrDefault(x => x.GetName() == selectedEngine || x.GetShortName() == selectedEngine);
            if (selectedWhiteEngine != null)
                MainUtils.VM.SelectedWhiteEngine = MainUtils.VM.Engines.FirstOrDefault(x => x.GetName() == selectedWhiteEngine || x.GetShortName() == selectedWhiteEngine);
            if (selectedBlackEngine != null)
                MainUtils.VM.SelectedBlackEngine = MainUtils.VM.Engines.FirstOrDefault(x => x.GetName() == selectedBlackEngine || x.GetShortName() == selectedBlackEngine);



            if (SelectedReflectionTimeInMinute!=0)
                Utils.SelectedReflectionTimeInMinute = SelectedReflectionTimeInMinute;
            if (selectedLevel != -1)
                _CPULevel = _whiteCPULevel = _blackCPULevel = MainUtils.DeepLevel = Utils.DeepLevel = selectedLevel;
            MainUtils.InitialDuration = 0;
            /*if (selectedDurationType != null)
            {
                if (selectedDurationType == "30mn")
                    MainUtils.InitialDuration = 30 * 60;
                if (selectedDurationType == "15mn")
                    MainUtils.InitialDuration = 15 * 60;
                if (selectedDurationType == "1h")
                    MainUtils.InitialDuration = 60 * 60;
            }*/

            var dateTimeNow = DateTime.Now;
            var posiblesMoveListSelectedPawn = new List<int>();

            var oldLocationIndex = -1;
            var isMove = false;

            if (objId == -1)//Initialisation
            {
                dateTimeNow = DateTime.Now;


                var initialVm = new DetailsViewModel(MainUtils.VM.MainBord);
                initialVm.SelectedLevel = Utils.DeepLevel;
                initialVm.SelectedReflectionTimeInMinute = Utils.SelectedReflectionTimeInMinute;
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
                    initialVm.BlackScore = (initialVm.MainBord.BlackScore - initialVm.MainBord.WhiteScore);
                else if (initialVm.MainBord.BlackScore < initialVm.MainBord.WhiteScore)
                    initialVm.WhiteScore = (initialVm.MainBord.WhiteScore - initialVm.MainBord.BlackScore);
                else
                    initialVm.BlackScore = initialVm.WhiteScore = 0;

                if (MainUtils.IsFullCPU)
                    initialVm.IsFullCPU = 1;
                else
                    initialVm.IsFullCPU = 0;
                if (!isFullCPU)
                {
                    //couleurs des levels
                    if (MainUtils.CPUColor == "B")
                    {
                        initialVm.StringBlackCPULevel = $"L {_CPULevel}";
                        initialVm.StringWhiteCPULevel = $"L {0}";
                    }
                    else
                    {
                        initialVm.StringWhiteCPULevel = $"L {_CPULevel}";
                        initialVm.StringBlackCPULevel = $"L {0}";
                    }
                }
                else
                {
                    initialVm.StringBlackCPULevel = $"L {_blackCPULevel}";
                    initialVm.StringWhiteCPULevel = $"L {_whiteCPULevel}";
                }

                //NOT modif
                return PartialView("Details", initialVm);
            }
            // NorthwindEntities entities = new NorthwindEntities();

            //SELECTION
            else if (MainUtils.FromGridIndex == -1)
            {
                MainUtils.FromGridIndex = objId;
                posiblesMoveListSelectedPawn = MainUtils.VM.MainBord.GetPossibleMoves(MainUtils.FromGridIndex, 0).Select(x => x.ToIndex).ToList();


            }
            //DEPLACEMENT
            else //if (MainUtils.CurrentTurnColor != MainUtils.CPUColor)
            {



                isMove = true;
                MainUtils.ToGridIndex = objId;

                var selectedPawn = MainUtils.VM.GetPawn(MainUtils.FromGridIndex);

                posiblesMoveListSelectedPawn = MainUtils.VM.MainBord.GetPossibleMoves(MainUtils.FromGridIndex, 0).Select(x => x.ToIndex).ToList();
                if (!posiblesMoveListSelectedPawn.Contains(MainUtils.ToGridIndex) || (selectedPawn.PawnColor != MainUtils.CurrentTurnColor))
                {
                    //si mouvement impossible, on ne fait rien
                    MainUtils.FromGridIndex = -1;
                    //return null;
                    dateTimeNow = DateTime.Now;
                    /*var mainBord = new Board();
                    mainBord.Init();*/
                    var vmOld = new DetailsViewModel(MainUtils.VM.MainBord);
                    vmOld.SelectedLevel = Utils.DeepLevel;
                    vmOld.SelectedReflectionTimeInMinute = Utils.SelectedReflectionTimeInMinute;
                    vmOld.DateTimeNow = dateTimeNow;
                    vmOld.CurrentTurn = MainUtils.CurrentTurnColor;
                    vmOld.ComputerColor = MainUtils.CPUColor;
                    vmOld.InitialDuration = MainUtils.InitialDuration;

                    //dans le cas de loaded
                    /* vmOld.HuntingBoardWhiteImageList = MainUtils.VM.HuntingBoardWhiteImageList;
                     vmOld.HuntingBoardBlackImageList = MainUtils.VM.HuntingBoardBlackImageList;
                     vmOld.MovingList = MainUtils.VM.MovingList;*/

                    MainUtils.MovingList = vmOld.MovingList;
                    if (MainUtils.CPUColor == "B")
                        vmOld.RevertWrapperClass = "revertWrapper";
                    if (vmOld.MainBord.WhiteScore < vmOld.MainBord.BlackScore)
                        vmOld.BlackScore = (vmOld.MainBord.BlackScore - vmOld.MainBord.WhiteScore);
                    else if (vmOld.MainBord.BlackScore < vmOld.MainBord.WhiteScore)
                        vmOld.WhiteScore = (vmOld.MainBord.WhiteScore - vmOld.MainBord.BlackScore);
                    else
                        vmOld.BlackScore = vmOld.WhiteScore = 0;
                    // return View(MainUtils.VM);
                    MainUtils.CaseList = vmOld.MainBord.GetCases().ToList();

                    if (MainUtils.IsFullCPU)
                        vmOld.IsFullCPU = 1;
                    else
                        vmOld.IsFullCPU = 0;

                    if (MainUtils.CPUColor == "B")
                    {
                        vmOld.StringBlackCPULevel = $"L {_CPULevel}";
                        vmOld.StringWhiteCPULevel = $"L {0}";
                    }
                    else
                    {
                        vmOld.StringWhiteCPULevel = $"L {_CPULevel}";
                        vmOld.StringBlackCPULevel = $"L {0}";
                    }
                    //re selection
                    return PartialView("Details", vmOld);

                }
                MainUtils.VM.MainBord.Move(MainUtils.FromGridIndex, MainUtils.ToGridIndex);
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
            var vm = new DetailsViewModel(MainUtils.VM.MainBord, MainUtils.FromGridIndex, posiblesMoveListSelectedPawn, movedOldLocationIndex, movedNewLocation);
            vm.SelectedLevel = Utils.DeepLevel;
            vm.SelectedReflectionTimeInMinute = Utils.SelectedReflectionTimeInMinute;

            vm.DateTimeNow = dateTimeNow;
            vm.FromGridIndex = MainUtils.FromGridIndex;
            vm.CurrentTurn = MainUtils.CurrentTurnColor;
            vm.ComputerColor = MainUtils.CPUColor;
            vm.InitialDuration = MainUtils.InitialDuration;
            MainUtils.MovingList = vm.MovingList;
            if (MainUtils.CPUColor == "B")
                vm.RevertWrapperClass = "revertWrapper";

            if (vm.MainBord.WhiteScore < vm.MainBord.BlackScore)
                vm.BlackScore = (vm.MainBord.BlackScore - vm.MainBord.WhiteScore);
            else if (vm.MainBord.BlackScore < vm.MainBord.WhiteScore)
                vm.WhiteScore = (vm.MainBord.WhiteScore - vm.MainBord.BlackScore);
            else
                vm.BlackScore = vm.WhiteScore = 0;
            // return View(MainUtils.VM);

            MainUtils.CaseList = vm.MainBord.GetCases().ToList();
            if (MainUtils.IsFullCPU)
                vm.IsFullCPU = 1;
            else
                vm.IsFullCPU = 0;
            //            GC.Collect();
            //couleurs des levels
            if (MainUtils.CPUColor == "B")
            {
                vm.StringBlackCPULevel = $"L {_CPULevel}";
                vm.StringWhiteCPULevel = $"L {0}";
            }
            else
            {
                vm.StringWhiteCPULevel = $"L {_CPULevel}";
                vm.StringBlackCPULevel = $"L {0}";
            }
            if(isMove)
                vm.IsMove = 1;
            //selection et déplacment
            return PartialView("Details", vm);
        }


        public FileResult SavePrintScreen(string image, string cases)
        {
            // var t_ = cases;
            List<string> caseList = JsonConvert.DeserializeObject<List<string>>(cases);

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
            //var caseList = pawnCases;//MainUtils.VM.Cases.ToList();
            if (caseList == null)
                return null;
            //  var caseListStr = String.Join("\n", caseList);
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

                var imageDestinationPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ToDownloadedFiles", $"{dirLocalPath}", $"{imageFileName}");

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
                System.IO.File.WriteAllText($"{dirLocalPath}/caseList.txt", cases);
                System.IO.File.WriteAllText($"{dirLocalPath}/WHITEList.txt", pawnStringWhite);
                System.IO.File.WriteAllText($"{dirLocalPath}/BLACKList.txt", pawnStringBlack);

                //historique
                var movingListStr = string.Empty;
                if (MainUtils.MovingList != null)
                    movingListStr = String.Join("\n", MainUtils.MovingList); //MainUtils.MovingList.Join( ("\\n");
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


            MainUtils.VM.MainBord.MovingList = MainUtils.MovingList;
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
            try
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
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return null;
            }




            //  return null;
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