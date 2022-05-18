 //Pour les test
    public Node ThreadGetBestMoveNotTask(string color)
    {

      try
      {
        System.GC.Collect();
        // GC.WaitForPendingFinalizers();

        // Node node = new Node();

        //Pour les BestsPosition

        if (SpecificPositionsList != null)
        {
          var specifiBoardList = new List<SpecificBoard>();
          foreach (var item in SpecificPositionsList)
          {
            var specificBoard = new SpecificBoard();

            specificBoard.Color = item.Color[0].ToString();
            specificBoard.Score = item.Score;
            specificBoard.SpecificsBoard = item.SpecificsBoard;
            specifiBoardList.Add(specificBoard);
          }
        }




        var boarChess2 = Chess2Utils.GenerateBoardFormPawnList(this.PawnList);

        //pour T82 NUll 2
        //  boarChess2.MovingList

        var engine = new Engine(DeeLevel, ComputerColore[0].ToString(), IsReprise, SpecifiBoardList,true);
        var firstInLastMove = GetTreeLastAction();

        var positions = firstInLastMove.Substring(firstInLastMove.Count() - 8, 8);
        positions = positions.Replace(" ", "");
        positions = positions.Replace("=", "");
        var positionsDatas = positions.Split('>');
        var fromIndexNotValide = Chess2Utils.GetIndexFromLocation(positionsDatas[0]);
        var toIndexNotValide = Chess2Utils.GetIndexFromLocation(positionsDatas[1]);

        var bestNodeChess2 = engine.Search(boarChess2, TurnNumberLabel.Content.ToString(), fromIndexNotValide, toIndexNotValide);

        Node node = new Node();
        node.Location = Chess2Utils.GetLocationFromIndex(bestNodeChess2.FromIndex);
        node.BestChildPosition = Chess2Utils.GetLocationFromIndex(bestNodeChess2.ToIndex);
        node.AssociatePawn = GetPawn(node.Location);
        //Seulment pour les test
        node.AsssociateNodeChess2 = bestNodeChess2;
        return node;
      }
      catch (Exception ex)
      {
        WriteInLog(ex.ToString());
        return null;
      }
    }