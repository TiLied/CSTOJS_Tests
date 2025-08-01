﻿using CSharpToJavaScript.APIs.JS;
using System;
using System.Collections.Generic;
using static CSharpToJavaScript.APIs.JS.Ecma.GlobalObject;

using Math = CSharpToJavaScript.APIs.JS.Ecma.Math;
using Object = CSharpToJavaScript.APIs.JS.Ecma.Object;

namespace UnitTestAnkiWebQuiz;

	public class AnkiWebQuiz
	{
		private dynamic _Options = new Object();
		private dynamic _Decks = new Object();
		private string _DeckId;

		public AnkiWebQuiz()
		{
			Console.WriteLine("AnkiWeb Quiz v" + GM.Info.Script.Version + " initialization");

			_LoadOptionsAndDecks();
			_SetCSS();
		}

		private void _SetCSS()
		{
			(GlobalThis.Window.Document.Head as ParentNode).Append("<!--Start of AnkiWeb Quiz v" + GM.Info.Script.Version + " CSS-->");
			
			
			GlobalThis.Window.Document.Head.InsertAdjacentHTML("beforeend", "<style type='text/css'>div.awq_quizGrid" +
			"{" +
				"display: grid;" +
				"grid-template-columns: repeat(4,auto);" +
				"grid-template-rows: auto;" +
			"}</style>");

			GlobalThis.Window.Document.Head.InsertAdjacentHTML("beforeend", "<style type='text/css'>div.awq_quizButton" +
			"{" +
				"color: #fff;" +
				"background-color: #0275d8;" +
				"border-color: #0275d8;" +
				"padding: .75rem 1.5rem;" +
				"font-size: 1rem;" +
				"border-radius: .3rem;" +
				"border: 1px solid transparent;" +
				"max-width:250px;" +
				"margin:5px;" +
				"cursor: pointer;" +
				"max-height: 300px;" +
				"overflow: auto;" +
			"}</ style >");
			GlobalThis.Window.Document.Head.InsertAdjacentHTML("beforeend", "<style type='text/css'>div.awq_quizButton:hover" +
			"{" +
				"background-color: #025aa5;" +
			"}</ style >");

			GlobalThis.Window.Document.Head.InsertAdjacentHTML("beforeend", "<style type='text/css'>div.awq_true" +
			"{" +
				"background-color: #75d802;" +
			"}</style>");
			GlobalThis.Window.Document.Head.InsertAdjacentHTML("beforeend", "<style type='text/css'>div.awq_true:hover" +
			"{" +
				"background-color: #5aa502;" +
			"}</style>");
			GlobalThis.Window.Document.Head.InsertAdjacentHTML("beforeend", "<style type='text/css'>.awq_trueBorder" +
			"{" +
				"border-color: #75d802;" +
			"}</style>");
			GlobalThis.Window.Document.Head.InsertAdjacentHTML("beforeend", "<style type='text/css'>div.awq_false" +
			"{" +
				"background-color: #d80275;" +
			"}</style>");
			GlobalThis.Window.Document.Head.InsertAdjacentHTML("beforeend", "<style type='text/css'>div.awq_false:hover" +
			"{" +
				"background-color: #a5025a;" +
			"}</style>");
			GlobalThis.Window.Document.Head.InsertAdjacentHTML("beforeend", "<style type='text/css'>.awq_falseBorder" +
			"{" +
				"border-color: #d80275;" +
			"}</style>");

			(GlobalThis.Window.Document.Head as ParentNode).Append("<!--End of AnkiWeb Quiz v" + GM.Info.Script.Version + " CSS-->");

		}

		private async void _LoadOptionsAndDecks() 
		{
			_Options = await GM.GetValue("awqOptions");
			_Decks = await GM.GetValue("awqDecks");

			if(_Options == null)
				_Options = new Object();
			if (_Decks == null)
				_Decks = new Object();

			//Console log prefs with value
			Console.WriteLine("*prefs:");
			Console.WriteLine("*-----*");

			List<dynamic> vals = await GM.ListValues();

			for (int i = 0; i < vals.Count; i++)
			{
				Console.WriteLine("*" + vals[i] + ":" + await GM.GetValue(vals[i]));
			}
			Console.WriteLine("*-----*");
		}

		public async void Main() 
		{
			if (GlobalThis.Window.Document.Location.Pathname.StartsWith("/decks"))
			{
				NodeList strs = (GlobalThis.Window.Document as ParentNode).QuerySelectorAll("button.btn-link");
				for (int i = 0; i < (int)strs.Length; i++)
				{
					string _id = (strs[i] as HTMLElement).Id;

					//Console.WriteLine(_id);

					if (_id.StartsWith("did")) 
					{
						if(_Decks[_id.Substring(3)] == null)
							_Decks[_id.Substring(3)] = new Object();

						(strs[i] as HTMLElement).AddEventListener("click", () => 
						{
							GM.SetValue("awqDeckId", _id.Substring(3));
						}, true);
					}
				}
				GM.SetValue("awqDecks", _Decks);
			}
			if (GlobalThis.Window.Document.Location.Pathname.StartsWith("/study"))
			{
				_DeckId = await GM.GetValue("awqDeckId");
				if (_DeckId == null) 
				{
					Console.WriteLine("Deck id is null");
					return;
				}

				dynamic _study = Eval("study");
				Console.WriteLine(_study);

				if (_study["currentCard"] == null) 
				{
					(GlobalThis.Window as WindowOrWorkerGlobalScope).SetTimeout(() =>
					{
						Main();
					}, 1000);
					return;
				}

				string _id = _study["currentCard"]["cardId"];
				
				_Decks[_DeckId][_id] = _study["currentCard"];

				for (int i = 0; i < _study["cards"].Count; i++)
				{
					_id = _study["cards"][i]["cardId"];
					_Decks[_DeckId][_id] = _study["cards"][i];
				}

				Qiuz(_study);

				GM.SetValue("awqDecks", _Decks);
				
			}
		}

		private void Qiuz(dynamic study) 
		{
			List<string> cardsId = new();

			cardsId.Add(study["currentCard"]["cardId"]);

			List<string> keys = Object.Keys(_Decks[_DeckId]);

			int len = 11;
			if(len >= keys.Count) 
				len = keys.Count - 1;

			for(int i = 0; i < len; i++) 
			{
				int _randomInt = GetRandomInt(keys.Count);
				string _id = keys[_randomInt];
				bool _continue = false;

				for (int j = 0; j < cardsId.Count; j++)
				{
					if (_id == cardsId[j])
					{
						i--;
						_continue = true;
						break;
					}
				}
				if (_continue)
					continue;
				else
					cardsId.Add(_id);
			}
			//Console.WriteLine(cardsId);

			cardsId = Shuffle(cardsId);

			Element before = (GlobalThis.Window.Document as ParentNode).QuerySelector("#qa_box");
			Element divGrid = GlobalThis.Window.Document.CreateElement("div");
			divGrid.ClassList.Add("awq_quizGrid");

			before.ParentNode.InsertBefore(divGrid, before);

			Element answer = (GlobalThis.Window.Document as ParentNode).QuerySelector("#ansbuta");
			if (!answer.ClassList.Contains("awqEvent")) 
			{
				answer.ClassList.Add("awqEvent");
				answer.AddEventListener("click", () =>
				{
					NodeList eases = (GlobalThis.Window.Document as ParentNode).QuerySelectorAll("#easebuts button");

					//Console.WriteLine(eases);
					for (int i = 0; i < (int)eases.Length; i++)
					{
						if ((eases[i] as Element).ClassList.Contains("awqEvent"))
							continue;

						(eases[i] as Element).ClassList.Add("awqEvent");
						eases[i].AddEventListener("click", () =>
						{
							AddEventsForEases();
						}, false);
					}
				}, false);
			}

			for (int i = 0; i < cardsId.Count; i++)
			{
				Element div = GlobalThis.Window.Document.CreateElement("div");
				div.ClassList.Add("awq_quizButton");
				div.Id = cardsId[i];

				div.AddEventListener("click", (e) => 
				{
					string _id = (e.CurrentTarget as Element).Id;
					Console.WriteLine(_id);

					Element _button = (GlobalThis.Window.Document as ParentNode).QuerySelector("#ansbuta");
					(_button as HTMLElement).Click();

					NodeList _eases = (GlobalThis.Window.Document as ParentNode).QuerySelectorAll("#easebuts button");

					//Console.WriteLine(_eases);
					for (int i = 0; i < (int)_eases.Length; i++)
					{
						if ((_eases[i] as Element).ClassList.Contains("awqEvent"))
							continue;

						(_eases[i] as Element).ClassList.Add("awqEvent");
						_eases[i].AddEventListener("click", () =>
						{
							AddEventsForEases();
						}, false);
					}

					if (_id == study["currentCard"]["cardId"])
					{
						div.ClassList.Add("awq_true");
						div.ClassList.Add("awq_trueBorder");
						
						(_eases[1] as Element).ClassList.Add("awq_trueBorder");
					}
					else 
					{
						div.ClassList.Add("awq_false");
						div.ClassList.Add("awq_falseBorder");
	
						(_eases[0] as Element).ClassList.Add("awq_falseBorder");
					}
				},false);

				string html = _Decks[_DeckId][cardsId[i]]["answer"].Replace(_Decks[_DeckId][cardsId[i]]["question"], "").Replace("\n\n<hr id=answer>\n\n", "").Replace("<img", "<img width=\"100%\"");

				div.InsertAdjacentHTML("beforeend", html);

				(divGrid as ParentNode).Append(div);
			}
		}

		private void AddEventsForEases() 
		{
			Element _grid = (GlobalThis.Window.Document as ParentNode).QuerySelector(".awq_quizGrid");
			(_grid as ChildNode).Remove();
			Main();
		}

		private int GetRandomInt(int max)
		{
			return Math.Floor(Math.Random() * max);
		}

		private List<string> Shuffle(List<string> array)
		{
			for (int i = array.Count - 1; i > 0; i--)
			{
				int _i = i + 1;
				int j = Math.Floor(Math.Random() * _i);
				string temp = array[i];
				array[i] = array[j];
				array[j] = temp;
			}

			return array;
		}
	}