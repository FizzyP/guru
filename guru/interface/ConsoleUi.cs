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
        const string kPromptSymbol = "> ";
        const string kMenuPromptSymbol = ">> ";


        public void parseArgs(string[] args)
		{
			if (args.Length == 0) {
				handleEmptyArgs();
				return;
			}
			
			int idx = 0;
            switch (args[0]) {

                case "prompt":
                case "p":
                    {
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

                case "add":
                case "a":
                    {
                        idx++;
                        parseAddCommand(args, ref idx);
                        return;
                    }

                case "chain":
                case "c":
                    {
                        idx++;
                        parseChainCommand(args, ref idx);
                        return;
                    }

                case "group":
                case "g":
                    {
                        idx++;
                        parseGroupCommand(args, ref idx);
                        return;
                    }


                case "remove":
                case "rm":
                    {
                        idx++;
                        parseRemoveCommand(args, ref idx);
                        return;
                    }

                case "rename":
                case "rn":
                    {
                        idx++;
                        parseRenameCommand(args, ref idx);
                        return;
                    }

                case "list":
                case "l":
                {
					idx++;
                    parseListCommand(args, ref idx);
					return;
				}

                case "tree":
                case "t":
                {
                    idx++;
                    parseTreeCommand(args, ref idx);
                    return;
                }

                case "skip":
                case "s":
                {
					idx++;
					handleSkipCommand(args, ref idx);
					return;
				}

                case "done":
                case "d":
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
							handleQuery(args[0], args, ref idx);
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

        #region Utils

        void consoleWriteHeader(string header)
        {
            Console.WriteLine("--- " + header + " ---");
        }



        int getUserInputFromNumericalMenu(string[] options)
        {
            for (int i = 0; i < options.Length; i++)
            {
                Console.WriteLine(i + "\t" + options[i]);
            }

            int result = -1;
            do
            {
                Console.Write(kMenuPromptSymbol);
                var input = Console.ReadLine();
                try
                {
                    result = (int)UInt32.Parse(input);
                    if (result < 0 || result >= options.Length)
                        throw new Exception(); // force us through the same code path as catch block

                    //  Yay something to return!
                    return result;
                }
                catch
                {
                    Console.WriteLine("Select an option by typing a number 0..." + (options.Length - 1) + ".");
                    continue;
                }

            }
            while (true);

        }

        #endregion

        #region Help

        void printHelp()
        {
            consoleWriteHeader("guru Help");
            displayAddCommandInstructions();
            displayChainCommandInstructions();
        }
        
        #endregion

        #region Query

        void handleQuery(string queryString, string[] args, ref int idx)
        {
            queryString = queryString.Substring(1);

            //	No args = print everything
            if (idx >= args.Length)
            {
                printAllItems();
                return;
            }

            if (queryString == "")
            {
                //	Interpret next string as regular expression
                var regexStr = args[idx];
                var regex = new Regex(regexStr);
                var matchingItems = ItemGroup.Items.findItemsWithDescriptionMatchingRegex(regex);
                consoleWriteHeader("Items with descriptions matching \"" + regexStr + "\"");
                foreach (var item in matchingItems)
                {
                    Console.WriteLine(item);
                }
            }
            else
            {
                Console.WriteLine("Unsupported query.");
            }
        }

        #endregion

        #region Comment

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
            Console.WriteLine("Added a comment to " + item);
        }


        void handleMethodOnItem(Item item, string[] args, ref int idx)
        {
            if (args.Length <= idx)
            {
                Console.WriteLine(item.ToDetailedString());
                return;
            }

            switch (args[idx])
            {
                case "dep":
                    {
                        idx++;
                        handleDepMethod(item, args, ref idx);
                        return;
                    }
                case "rmdep":
                    {
                        idx++;
                        handleRmdepMethod(item, args, ref idx);
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
                default:
                    {
                        Console.WriteLine("Unrecognized item method '" + args[idx] + "'.");
                        return;
                    }
            }
        }

        #endregion

        #region Add/Remove Dependencies

        void handleDepMethod(Item item, string[] args, ref int idx)
        {
            if (args.Length <= idx)
            {
                printDepMethodErrorMessage();
                return;
            }

            try
            {
                var id = UInt64.Parse(args[idx]);
                var item2 = ItemGroup.getItemById(id);
                addDependency(item, item2);
                ItemGroupFile.save();
                return;
            }
            catch
            {
                printDepMethodErrorMessage();
                return;
            }
        }

        void addDependency(Item item1, Item item2)
        {
            try
            {
                ItemGroup.Items.addDependency(item1, item2);
            }
            catch (CircularDependencyException ex)
            {
                Console.WriteLine("Making " + item1.Id + " dependent on " + item2.Id + " would create a circular dependency.");
                return;
            }
            printAddDepMethodMessage(item1, item2);
        }

        void handleRmdepMethod(Item item, string[] args, ref int idx)
        {
            if (args.Length <= idx)
            {
                printNodepMethodErrorMessage();
                return;
            }

            try
            {
                var id = UInt64.Parse(args[idx]);
                var item2 = ItemGroup.getItemById(id);
                var didRemove = ItemGroup.Items.removeDependency(item, item2);
                if (didRemove)
                    Console.WriteLine("Item #" + item.Id + " is no longer dependent on item #" + item2.Id + ".");
                else
                    Console.WriteLine("Item #" + item.Id + " was not dependent on item #" + item2.Id + ".");

                ItemGroupFile.save();
                return;
            }
            catch
            {
                printNodepMethodErrorMessage();
                return;
            }
        }
        void printAddDepMethodMessage(Item item1, Item item2)
        {
            Console.WriteLine("Item #" + item1.Id + " is now dependent on item #" + item2.Id + ".");
        }

        void printDepMethodErrorMessage()
        {
            Console.WriteLine("Example: guru 1 dep 2");
        }

        void printNodepMethodErrorMessage()
        {
            Console.WriteLine("Example: guru 1 rmdep 2");
        }

        #endregion

        #region Prompt

        void enterPromptLoop()
        {
            string command;
            while (true)
            {
                Console.WriteLine();
                Console.Write(kPromptSymbol);
                command = Console.ReadLine();
                if (command == "quit" || command == "q")
                    return;

                //  Split the command up just like the shell would do and pass it to the argument parser
                string[] splitCommand = CommandLineStringSplitter.SplitCommandLine(command).ToArray();
                parseArgs(splitCommand);
            }
        }

        #endregion

        #region Next

        void parseNextCommand(string[] args, ref int idx)
		{
            handleNextItem();
        }

        void handleNextItem()
        {
            var nextItem = ItemGroup.Items.getTopItem();
            if (nextItem != null)
            {
                consoleWriteHeader("Next Item");
                Console.WriteLine(nextItem.ToDetailedString());
            }
            else
            {
                //	There's no task to do!
                if (ItemGroup.Items.ItemCount == 0)
                {
                    if (ItemGroup.Items.DoneItemCount == 0)
                    {
                        Console.WriteLine("There are no pending items.");
                    }
                    else
                    {
                        Console.WriteLine("There are no pending items.  All items are done.");
                    }
                }
                else
                {
                    //	There are pending items but they're not "free" which means
                    //	they're tied up in circular dependency.
                    Console.WriteLine("All pending items have unresolved dependencies.");
                }
                return;
            }
        }

        #endregion

        #region Remove

        void parseRemoveCommand(string[] args, ref int idx)
        {
            var item = ItemGroup.Items.getTopItem();

            bool forceRemoveDependencies = false;

            //  Look for dependencies on this item
            var deps = ItemGroup.Items.getDependents(item);
            if (deps.Count != 0)
            {
                consoleWriteHeader("The following items are dependent on the one you want to remove");
                foreach (var i in deps)
                    Console.WriteLine(i);
                consoleWriteHeader("What do you want to do?")
                var resultIdx = getUserInputFromNumericalMenu(
                    new string[]
                    {
                        "Remove the dependencies",
                        "Cancel"
                    }
                    );

                switch (resultIdx)
                {
                    case 0:
                        forceRemoveDependencies = true;
                        break;

                    case 1:
                        return;
                }


                ItemGroup.Items.removeItem(item, forceRemoveDependencies);

                ItemGroupFile.save();
                Console.WriteLine("Removed item " + item);
            }
        }

#endregion

        #region Add

        Item parseAddCommand(string[] args, ref int idx)
		{
			if (args.Length <= idx) {
				Console.WriteLine("Expected description.");
				displayAddCommandInstructions();
                return null;
			}
			var description = args[ idx++ ];
			
			var newId = ItemGroup.allocateId();
			var item = new Item( newId, description );

            addItem(item);
            this.ItemGroupFile.save();
            return item;
		}

        void addItem(Item item)
        {
            ItemGroup.Items.addItem(item);
            Console.WriteLine("Added item " + item);
        }

        void displayAddCommandInstructions()
		{
			Console.WriteLine("Command: 'add'");
			Console.WriteLine("Example: guru add \"Task description\"");
		}

        #endregion

        #region Chain

        //  If x is a number, finds the task with that id
        //      returns null if it doesn't exist.
        //  If x is a string makes a new task with that description
        Item createOrGetItemFromIdOrString(string x)
        {
            Item newItem;

            //  Extract an id or description
            UInt32 itemId;
            if (UInt32.TryParse(x, out itemId))
            {
                //  It's a task id
                newItem = ItemGroup.Items.getItemById(itemId);
                //  Assume newItem isn't null because we checked above
            }
            else
            {
                //  Createa a new item based on description and add it to the item group
                newItem = new Item(ItemGroup.allocateId(), x);
                addItem(newItem);
            }

            return newItem;
        }

        //  Returns -1 if ieverything is good.
        //  Returns index of invalid id otherwise
        int argListContainsUnknownIds(string[] args, int idx)
        {
            //  Test to make sure all the numbers appearing are known item ids
            int startIdx = idx;
            for (int i = startIdx; i < args.Length; i++)
            {
                //  Extract an id or description
                UInt32 itemId;
                if (UInt32.TryParse(args[i], out itemId))
                {
                    //  It's a task id
                    var newItem = ItemGroup.Items.getItemById(itemId);
                    if (newItem == null)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }


        void parseChainCommand(string[] args, ref int idx)
        {
            if (args.Length <= idx)
            {
                Console.WriteLine("Expected a sequence of item numbers or new item descriptions.");
                displayChainCommandInstructions();
                return;
            }

            var badIdx = argListContainsUnknownIds(args, idx);
            if (badIdx != -1)
            {
                Console.WriteLine("The number " + args[badIdx] + " is not a valid item id.  No tasks/dependencies created.");
                return;
            }

            //  Things look good, make a list of all the items to be chained
            List<Item> itemsToChain = new List<Item>();
            var startIdx = idx;
            for (int i = startIdx; i < args.Length; i++)
            {
                Item newItem = createOrGetItemFromIdOrString(args[i]);
                itemsToChain.Add(newItem);
            }

            //  Chain dependencies
            for (int i=0; i < itemsToChain.Count - 1; i++)
            {
                addDependency(itemsToChain[i], itemsToChain[i + 1]);
            }
                

            this.ItemGroupFile.save();
            Console.WriteLine("Chain complete");
        }

        void displayChainCommandInstructions()
        {
            Console.WriteLine("Command: chain");
            Console.WriteLine("Syntax: guru chain [item id | new item description]*");
            Console.WriteLine("Makes each item dependent on its successor.");
        }

        #endregion

        #region Group

        void parseGroupCommand(string[] args, ref int idx)
        {
            if (args.Length <= idx)
            {
                Console.WriteLine("Expected a sequence of item numbers or new item descriptions.");
                displayGroupCommandInstructions();
                return;
            }

            var badIdx = argListContainsUnknownIds(args, idx);
            if (badIdx != -1)
            {
                Console.WriteLine("The number " + args[badIdx] + " is not a valid item id.  No tasks/dependencies created.");
                return;
            }

            //  Things look good, make a list of all the items to be chained
            var firstItem = createOrGetItemFromIdOrString(args[idx]);
            List<Item> subItems = new List<Item>();
            var startIdx = idx + 1;
            for (int i = startIdx; i < args.Length; i++)
            {
                Item newItem = createOrGetItemFromIdOrString(args[i]);
                subItems.Add(newItem);
            }

            //  Add dependencies
            for (int i = 0; i < subItems.Count; i++)
            {
                addDependency(firstItem, subItems[i]);
            }

            this.ItemGroupFile.save();
            Console.WriteLine("Grouping complete.");
        }

        void displayGroupCommandInstructions()
        {
            Console.WriteLine("Command: group");
            Console.WriteLine("Syntax: guru group [item id | new item description]*");
            Console.WriteLine("Makes the first item dependent all following items.");
        }

        #endregion

        #region Rename

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

        #endregion

        #region Reopen

        void handleReopenMethod(Item item, string[] args, ref int idx)
        {
            if (idx != args.Length)
            {
                Console.WriteLine("Expected end of command.  Found \"" + args[idx] + "\" instead.");
                return;
            }

            handleReopenItem(item);
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

        #endregion

        #region Done

        void handleDoneMethod(Item item, string[] args, ref int idx)
        {
            if (idx != args.Length)
            {
                Console.WriteLine("Expected end of command.  Found \"" + args[idx] + "\" instead.");
                return;
            }
            handleDoneItem(item);
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
            handleNextItem();
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
        #endregion

        #region Skip

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
                    "The item is too complicated.",
                    "There are more important things to do."
                }
                );

            switch (resultIdx)
            {
                case 0:
                    handleNotInMoodForItem(item);
                    break;
                case 1:
                    handleProcrastinatingOnItem(item);
                    break;
                case 2:
                    handleSomethingElseToBeDoneFirst(item);
                    break;
                case 3:
                    handleItemTooComplicated(item);
                    break;
                case 4:
                    handleLowPriority(item);
                    break;
            }
        }

        void handleNotInMoodForItem(Item item)
        {
            Console.WriteLine("Quit whining.  (feature currently unspported).");
        }

        void handleProcrastinatingOnItem(Item item)
        {
            Console.WriteLine("Quit whining.  (feature currently unspported).");
        }

        void handleSomethingElseToBeDoneFirst(Item item)
        {
            Console.WriteLine("What needs to be done first?");
            Console.WriteLine("Enter an item number, an add command, ? to search, or 'q' to quit.");

            Item item2 = null;
            while (true)
            {
                Console.Write(kMenuPromptSymbol);
                var input = Console.ReadLine();

                if (input == "q")
                {
                    Console.WriteLine("That's what I thought.  Get back to work.");
                    return;
                }

                //  See if it's an item number
                UInt32 item2Id;
                if (UInt32.TryParse(input, out item2Id))
                {
                    item2 = ItemGroup.Items.getItemById(item2Id);
                    if (item2 == null)
                    {
                        Console.WriteLine("There is no item with that number.");
                        continue;
                    }
                    else
                    {
                        // generate the dependency when we exit the input loop.
                        break;
                    }
                }

                //  Break into words and see if its add or ?
                var inputSplit = CommandLineStringSplitter.SplitCommandLine(input).ToArray<String>();
                if (inputSplit.Length == 0)
                    continue;

                if (inputSplit[0][0] == '?')
                {
                    int idx = 1;
                    handleQuery(inputSplit[0], inputSplit, ref idx);
                }
                else if (inputSplit[0] == "add")
                {
                    int idx = 1;
                    item2 = parseAddCommand(inputSplit, ref idx);
                    if (item2 == null)
                        continue;

                    //  If they really created an item then assign it
                    //  to item2 and generate the dependency when we exit the input loop.
                    break;
                }
            }

            ItemGroup.Items.addDependency(item, item2);
            printAddDepMethodMessage(item, item2);
            ItemGroupFile.save();
        }

        void handleItemTooComplicated(Item item)
        {
            Console.WriteLine("Let's break up this item into smaller peices.");
            Console.WriteLine("Enter an item number to make it a sub-item, use 'add' to create a new a sub-item, ? to search, or 'q' to quit.");

            bool didAddSubtask = false;

            Item item2 = null;
            while (true)
            {
                Console.Write(kMenuPromptSymbol);
                var input = Console.ReadLine();

                if (input == "q")
                {
                    if (didAddSubtask)
                        Console.WriteLine("Congrats!  You have subdivided your task.");
                    else
                        Console.WriteLine("Warning: you haven't subdivided your task.");

                    return;
                }

                //  See if it's an item number
                UInt32 item2Id;
                if (UInt32.TryParse(input, out item2Id))
                {
                    item2 = ItemGroup.Items.getItemById(item2Id);
                    if (item2 == null)
                    {
                        Console.WriteLine("There is no item with that number.");
                        continue;
                    }
                    else
                    {
                        ItemGroup.Items.addDependency(item, item2);
                        printAddDepMethodMessage(item, item2);
                        ItemGroupFile.save();
                        didAddSubtask = true;
                        continue;
                    }
                }

                //  Break into words and see if its add or ?
                var inputSplit = CommandLineStringSplitter.SplitCommandLine(input).ToArray<String>();
                if (inputSplit.Length == 0)
                    continue;

                if (inputSplit[0][0] == '?')
                {
                    int idx = 1;
                    handleQuery(inputSplit[0], inputSplit, ref idx);
                }
                else if (inputSplit[0] == "add")
                {
                    int idx = 1;
                    item2 = parseAddCommand(inputSplit, ref idx);
                    if (item2 == null)
                        continue;

                    ItemGroup.Items.addDependency(item, item2);
                    printAddDepMethodMessage(item, item2);
                    ItemGroupFile.save();
                    didAddSubtask = true;
                    continue;
                }
            }

        }

        void handleLowPriority(Item item)
        {
            double kLowRankWarningThreshold = -2;
            if (item.Rank < kLowRankWarningThreshold)
            {
                consoleWriteHeader("You Keep Deprioritizing This item");
                var resultIdx = getUserInputFromNumericalMenu(
                    new string[]
                    {
                    "Trust Me",
                    "I'm just procrastinating",
                    "I'm never going to do this, remove it"
                    }
                    );

                switch (resultIdx)
                {
                    case 1:
                        Console.WriteLine("Quit whining.  (feature currently unspported).");
                        return;

                    case 2:
                        Console.WriteLine("Quit whining.  (feature currently unspported).");
                        return;

                    default:
                        //  Don't do anything.  Fall through to decreasing priority
                        break;
                }
            }
            ItemGroup.Items.setItemRank(item, item.Rank - 1);
            ItemGroupFile.save();
            Console.WriteLine("Decreased the priority of " + item);
        }


        #endregion

        #region Tree

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

        #endregion

        #region List

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

        #endregion

    }
}

