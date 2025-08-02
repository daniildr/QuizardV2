# Quizard

Для визуализации схемы переходов необходимо скопировать схему в формате .dot и визуализировать ее с помощью [WebGraphviz](http://www.webgraphviz.com/).

### Схема в формате [.mmd](fsm.mmd)
### Схема в формате .dot:
```dot
digraph {
compound=true;
node [shape=Mrecord]
rankdir="LR"
"NotStarted" [label="NotStarted"];
"Pause" [label="Pause|exit / Function"];
"WaitingForPlayers" [label="WaitingForPlayers|exit / Function"];
"Media" [label="Media|entry / Function"];
"RoundPlaying" [label="RoundPlaying|entry / Function"];
"QuestionPlaying" [label="QuestionPlaying|entry / Function"];
"RevealShowing" [label="RevealShowing|entry / Function\nexit / Function"];
"Auction" [label="Auction|entry / Function\nexit / Function"];
"WaitStats" [label="WaitStats|entry / Function\nexit / Function"];
"ShowingStats" [label="ShowingStats|entry / Function\nexit / Function"];
"Voting" [label="Voting|entry / Function\nexit / Function"];
"Shop" [label="Shop|entry / Function"];
"ApplyingTargetModifiers" [label="ApplyingTargetModifiers|entry / Function\nexit / Function"];
"Finished" [label="Finished|entry / Function\nexit / Function"];
"ShowingScenarioStats" [label="ShowingScenarioStats|entry / Function"];
"Decision1" [shape = "diamond", label = "Function"];
"Decision2" [shape = "diamond", label = "Function"];
"Decision3" [shape = "diamond", label = "Function"];
"Decision4" [shape = "diamond", label = "Function"];
"Decision5" [shape = "diamond", label = "Function"];
"Decision6" [shape = "diamond", label = "Function"];
"Decision7" [shape = "diamond", label = "Function"];
"Decision8" [shape = "diamond", label = "Function"];
"Decision9" [shape = "diamond", label = "Function"];
"Decision10" [shape = "diamond", label = "Function"];
"Decision11" [shape = "diamond", label = "Function"];
"Decision12" [shape = "diamond", label = "Function"];
"Decision13" [shape = "diamond", label = "Function"];
"Decision14" [shape = "diamond", label = "Function"];
"Decision15" [shape = "diamond", label = "Function"];
"Decision16" [shape = "diamond", label = "Function"];
"Decision17" [shape = "diamond", label = "Function"];
"Decision18" [shape = "diamond", label = "Function"];
"Decision19" [shape = "diamond", label = "Function"];
"Decision20" [shape = "diamond", label = "Function"];
"Decision21" [shape = "diamond", label = "Function"];

"NotStarted" -> "WaitingForPlayers" [style="solid", label="StartRequested"];
"Pause" -> "Finished" [style="solid", label="EndRequested"];
"Pause" -> "Decision1" [style="solid", label="ResumeRequested"];
"WaitingForPlayers" -> "WaitingForPlayers" [style="solid", label="PlayerIdentified"];
"WaitingForPlayers" -> "Decision2" [style="solid", label="AllPlayersReady"];
"WaitingForPlayers" -> "Decision3" [style="solid", label="PauseRequested"];
"Media" -> "Finished" [style="solid", label="EndRequested"];
"Media" -> "Decision4" [style="solid", label="Skip"];
"Media" -> "Decision5" [style="solid", label="MediaEnded"];
"Media" -> "Decision6" [style="solid", label="PauseRequested"];
"RoundPlaying" -> "QuestionPlaying" [style="solid", label="Skip"];
"RoundPlaying" -> "QuestionPlaying" [style="solid", label="RoundStarted"];
"RoundPlaying" -> "Auction" [style="solid", label="AuctionStarted"];
"RoundPlaying" -> "Finished" [style="solid", label="EndRequested"];
"RoundPlaying" -> "Decision7" [style="solid", label="PauseRequested"];
"RoundPlaying" -> "RoundPlaying" [style="solid", label="ApplyTargetModifiersCompleted"];
"QuestionPlaying" -> "RevealShowing" [style="solid", label="Skip"];
"QuestionPlaying" -> "Finished" [style="solid", label="EndRequested"];
"QuestionPlaying" -> "RevealShowing" [style="solid", label="RoundTimeout / Function"];
"QuestionPlaying" -> "Decision8" [style="solid", label="QuestionCompleted"];
"QuestionPlaying" -> "Decision9" [style="solid", label="PauseRequested"];
"RevealShowing" -> "Finished" [style="solid", label="EndRequested"];
"RevealShowing" -> "Decision10" [style="solid", label="Skip"];
"RevealShowing" -> "Decision11" [style="solid", label="RevealShowed"];
"RevealShowing" -> "Decision12" [style="solid", label="PauseRequested"];
"Auction" -> "Finished" [style="solid", label="EndRequested"];
"Auction" -> "QuestionPlaying" [style="solid", label="AuctionCompleted"];
"Auction" -> "Decision13" [style="solid", label="PauseRequested"];
"WaitStats" -> "ShowingStats" [style="solid", label="StatsRequested"];
"WaitStats" -> "Finished" [style="solid", label="EndRequested"];
"WaitStats" -> "Decision14" [style="solid", label="PauseRequested"];
"WaitStats" -> "WaitStats" [style="solid", label="RevealShowed"];
"ShowingStats" -> "Finished" [style="solid", label="EndRequested"];
"ShowingStats" -> "Decision15" [style="solid", label="Skip"];
"ShowingStats" -> "Decision16" [style="solid", label="StatsDisplayed"];
"ShowingStats" -> "Decision17" [style="solid", label="PauseRequested"];
"Voting" -> "Finished" [style="solid", label="EndRequested"];
"Voting" -> "Decision18" [style="solid", label="VotingCompleted"];
"Voting" -> "Decision19" [style="solid", label="PauseRequested"];
"Shop" -> "ApplyingTargetModifiers" [style="solid", label="Skip"];
"Shop" -> "ApplyingTargetModifiers" [style="solid", label="ShopEnded"];
"Shop" -> "ApplyingTargetModifiers" [style="solid", label="ShopTimeout"];
"Shop" -> "Finished" [style="solid", label="EndRequested"];
"Shop" -> "Decision20" [style="solid", label="PauseRequested"];
"ApplyingTargetModifiers" -> "Finished" [style="solid", label="EndRequested"];
"ApplyingTargetModifiers" -> "Decision21" [style="solid", label="ApplyTargetModifiersCompleted"];
"Finished" -> "ShowingScenarioStats" [style="solid", label="StatsRequested"];
 init [label="", shape=point];
 init -> "NotStarted"[style = "solid"]
}
```