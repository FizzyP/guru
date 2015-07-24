using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;

namespace Guru
{
	class ConsoleUi
	{
		const string DefaultItemGroupFilePath = "fizzy.guru";

		public void parseArgs(string[] args)
		{
			if (args.Length == 0) {
				handleEmptyArgs();
				return;
			}
			
			int idx = 0;
            switch (args[0]) {

                case ">": {
                        enterPromptLoop();
                        break;
                    }

                case "help": {
                        printHelp();
                        return;
                    }

                case "next": {
                        idx++;
                        parseNextCommand(args, ref idx);
                        return;
                    }

                case "add": {
                        idx++;
                        parseAddCommand(args, ref idx);
                        return;
                    }

                case "rename": {
                        idx++;
                        parseRenameCommand(args, ref idx);
                        return;
                    }

                case "list": {
					idx++;
                    parseListCommand(args, ref idx);
					return;
				}

                case "tree":
                {
                    idx++;
                    parseTreeCommand(args, ref idx);
                    return;
                }

                case "skip": {
					idx++;
					handleSkipCommand(args, ref idx);
					return;
				}

                case "done":
                {
                    idx++;
                    handleDoneCommand(args, ref idx);
                    return;
                }

                case "comment":
                {
                    idx++;
                    handleCommentCommand(args, ref idx);
                    return;
                }

                default: {
					if (args[0].Length > 0) {
						
						//	Queries
						if (args[0][0] == '?') {
							idx++;
							handleQuery(args[0].Substring(1), args, ref idx);
							return;
						}
						else {
							//	Methods on Items
							var idStr = args[0];
							try {
								UInt64 idNum = UInt64.Parse( idStr );
								var item = ItemGroup.getItemById(idNum);
								idx++;
								handleMethodOnItem(item, args, ref idx);
								return;
							}
							catch {
								Console.WriteLine("Expecting a keyword or an item id number.  Found \"" + args[0] + "\" instead.");
								return;							
							}
						}
					}
					return;
				}
			}
		}

		void handleSkipCommand(string[] args, ref int idx)
		{
            var item = ItemGroup.Items.getTopItem();

            Console.WriteLine("So you want to skip " + item);
            consoleWriteHeader("What's your Excuse?");

            var resultIdx = getUserInputFromNumericalMenu(
                new string[]
                {
                    "I'm not in the mood.",
                    "I'm procrastinating",
                    "Something else needs to be done first.",
                    "The item is too complicated."
                }
                );

            
        }


        int getUserInputFromNumericalMenu(string[] options)
        {
            for (int i = 0; i < options.Length; i++)
            {
                Console.WriteLine(i + "\t" + options[i]);
            }

            int result = -1;
            do {
                var input = Console.ReadLine();
                try
                {
                    result = (int) UInt32.Parse(input);
                    if (result < 0 || result >= options.Length)
                        throw new Exception(); // force us through the same code path as catch block

                    //  Yay something to return!
                    return result;
                }
                catch
                {
                    Console.WriteLine("Select an option by typing a number 0..." + (options.Length-1) + ".");
                    continue;
                }

            }
            while (true) ;

        }

        void parseTreeCommand(string[] args, ref int idx)
        {
            if (idx == args.Count())
            {
                consoleWriteHeader("Item Tree");
                Console.WriteLine(ItemGroup.Items.getItemsAsForest().getTreeFormattedString());
                return;
            }

            switch (args[idx])
            {
                case "done":
                    {
                        consoleWriteHeader("Done Item Tree");
                        Console.WriteLine(ItemGroup.Items.getDoneItemsAsForest().getTreeFormattedString());
                        return;
                    }
                default:
                    {
                        Console.WriteLine("Unrecognized item method \"" + args[idx] + "\"");
                        return;
                    }
            }
        }


        void parseListCommand(string[] args, ref int idx)
        {
            if (idx == args.Count())
            {
                printAllItems();
                return;
            }

            switch (args[idx])
            {
                case "done":
                    {
                        consoleWriteHeader("Done Items");
                        printItems(ItemGroup.EnumerableDoneItems);
                        return;
                    }
                default:
                    {
                        Console.WriteLine("Unrecognized item method \"" + args[idx] + "\"");
                        return;
                    }
            }
        }


        void printAllItems()
        {
            consoleWriteHeader("Items");
            printItems(ItemGroup.EnumerableItems);
        }

        void printItems(IEnumerable<Item> items)
        {
            foreach (var item in items)
            {
                Console.WriteLine(item);
            }
        }



        void handleDoneCommand(string[] args, ref int idx)
        {
            var doneItem = ItemGroup.Items.getTopItem();
            if (doneItem == null)
            {
                Console.WriteLine("You said you were done but there are no items to finish!  Add some items first.");
                return;
            }

            if (idx != args.Length)
            {
                Console.WriteLine("Expected end of command but found \"" + args[idx] + "\" instead.");
                return;
            }

            handleDoneItem(doneItem);
        }

        void handleDoneItem(Item doneItem)
        {
            //  Finish the item
            ItemGroup.Items.items.Remove(doneItem);
            doneItem.IsDone = true;
            ItemGroup.Items.doneItems.Add(doneItem);
            Console.WriteLine("Completed item " + doneItem);

            ItemGroupFile.save();
        }

        void handleReopenItem(Item item)
        {
            //  Finish the item
            ItemGroup.Items.doneItems.Remove(item);
            item.IsDone = false;
            ItemGroup.Items.items.Add(item);
            Console.WriteLine("Reopened item " + item);

            ItemGroupFile.save();
        }

        void handleQuery(string queryString, string[] args, ref int idx)
		{
			//	No args = print everything
			if (idx >= args.Length) {
				printAllItems();
				return;
			}
					
			if (queryString == "") {
				//	Interpret next string as regular expression
				var regexStr = args[idx];
				var regex = new Regex(regexStr);
				var matchingItems = ItemGroup.Items.findItemsWithDescriptionMatchingRegex( regex );
				consoleWriteHeader("Items with descriptions matching \"" + regexStr + "\"");
				foreach (var item in matchingItems) {
					Console.WriteLine(item);
				}
			}
			else {
				Console.WriteLine("Unsupported query.");
			}
		}

        void handleCommentCommand(string[] args, ref int idx)
        {
            if (args.Length == idx)
            {
                Console.WriteLine("Comment command has format: comment \"comment text\"");
                return;
            }

            var comment = args[idx];
            var item = ItemGroup.Items.getTopItem();
            handleCommentOnItem(item, comment);
        }

        void handleCommentOnItem(Item item, string comment)
        {
            item.Details += comment + System.Environment.NewLine;
            ItemGroupFile.save();
        }


            void handleMethodOnItem(Item item, string[] args, ref int idx)
		{
			if (args.Length <= idx) {
				Console.WriteLine(item.ToDetailedString());
				return;
			}
			
			switch (args[idx]) {
				case "dep": {
					idx++;
					handleDepMethod(item, args, ref idx);
					return;
				}
                case "done":
                {
                    idx++;
                    handleDoneMethod(item, args, ref idx);
                    return;
                }
                case "reopen":
                {
                    idx++;
                    handleReopenMethod(item, args, ref idx);
                    return;
                }
				default: {
					Console.WriteLine("Unrecognized item method '" + args[idx] + "'.");
					return;
				}
			}
		}

        void handleReopenMethod(Item item, string[] args, ref int idx)
        {
            if (idx != args.Length)
            {
                Console.WriteLine("Expected end of command.  Found \"" + args[idx] + "\" instead.");
                return;
            }

            handleReopenItem(item);
        }

        void handleDoneMethod(Item item, string[] args, ref int idx)
        {
            if (idx != args.Length)
            {
                Console.WriteLine("Expected end of command.  Found \"" + args[idx] + "\" instead.");
                return;
            }
            handleDoneItem(item);
        }


        void handleDepMethod(Item item, string[] args, ref int idx)
		{
			if (args.Length <= idx) {
				printDepMethodErrorMessage();
				return;
			}	
			
			try {
				var id = UInt64.Parse( args[idx] );
				var item2 = ItemGroup.getItemById( id );
				ItemGroup.Items.addDependency(item, item2);
                ItemGroupFile.save();
				//  Console.WriteLine(item);
				//  Console.WriteLine(item2);
				Console.WriteLine("Item #" + item.Id + " is now dependent on item #" + item2.Id + ".");
				return;
			}
			catch {
				printDepMethodErrorMessage();
				return;
			}
		}
		
		void printDepMethodErrorMessage()
		{
//			Console.WriteLine("'dep' method expects the # of the dependency.");
			Console.WriteLine("Example: guru #1 dep #2");
		}
		
		
		void handleEmptyArgs()
		{
			//	Load it
			var groupFile = ItemGroupFile;
			
			if (ItemGroupFileIsNew) {
				printHelp();
                ItemGroupFileIsNew = false;
			}
			else {
				int idx = 0;
				parseNextCommand(new string[0], ref idx);
			}
		}
		
		void printHelp()
		{
			consoleWriteHeader("guru Help");
			displayAddCommandInstructions();
		}
		
		
		
		void consoleWriteHeader(string header) {
			Console.WriteLine("--- " + header + " ---");
		}
		
		void enterPromptLoop()
        {
            string command;
            while (true)
            {
                Console.WriteLine();
                Console.Write("> ");
                command = Console.ReadLine();
                if (command == "quit" || command == "q")
                    return;

                //  Split the command up just like the shell would do and pass it to the argument parser
                string[] splitCommand = CommandLineStringSplitter.SplitCommandLine(command).ToArray();
                parseArgs(splitCommand);
            }
        }



        #region Default Item Group

        ItemGroup ItemGroup {
			get {
				return ItemGroupFile.ItemGroup;				
			}
		}
		
		bool ItemGroupFileIsNew = false;
		private ItemGroupFile _itemGroupFile;
		ItemGroupFile ItemGroupFile {
			get {
				if (_itemGroupFile == null) {
					if (File.Exists(DefaultItemGroupFilePath)) {					
						_itemGroupFile = ItemGroupFile.new_ReadFromPath(DefaultItemGroupFilePath);
						ItemGroupFileIsNew = false;
					}
					else {
						_itemGroupFile = ItemGroupFile.new_CreateAtPath(DefaultItemGroupFilePath);
						ItemGroupFileIsNew = true;
					}
				}
				return _itemGroupFile;
			}
		}

		#endregion




		#region next command
		
		void parseNextCommand(string[] args, ref int idx)
		{
			var nextItem = ItemGroup.Items.getTopItem();
			if (nextItem != null) {
                consoleWriteHeader("Next Item");
				Console.WriteLine( nextItem );
			}		
			else {
				//	There's no task to do!
				if (ItemGroup.Items.ItemCount == 0) {
					if (ItemGroup.Items.DoneItemCount == 0) {
						Console.WriteLine("There are no pending items.");
					}
					else {
						Console.WriteLine("There are no pending items.  All items are done.");
					}
				}
				else {
					//	There are pending items but they're not "free" which means
					//	they're tied up in circular dependency.
					Console.WriteLine("All pending items have unresolved dependencies.");
				}
				return;
			}
		}
			
		#endregion	




		
		#region add command
		
		void parseAddCommand(string[] args, ref int idx)
		{
			if (args.Length <= idx) {
				Console.WriteLine("Expected description.");
				displayAddCommandInstructions();
				return;
			}
			var description = args[ idx++ ];
			
			var newId = ItemGroup.allocateId();
			var item = new Item( newId, description );
			
			ItemGroup.Items.addItem( item );
			this.ItemGroupFile.save();
			Console.WriteLine("Added item " + item);
		}
		
		void displayAddCommandInstructions()
		{
			Console.WriteLine("Command: 'add'");
			Console.WriteLine("Example: guru add \"Task description\"");
		}

        #endregion

        void parseRenameCommand(string[] args, ref int idx)
        {
            if (args.Length == idx)
            {
                Console.WriteLine("rename command format: rename \"new name\".");
                return;
            }

            var newName = args[idx];
            var item = ItemGroup.Items.getTopItem();
            handleRenameItem(item, newName);
        }

        void handleRenameItem(Item item, string newName)
        {
            item.Description = newName;
            ItemGroupFile.save();
        }

    }
}

