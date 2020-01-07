using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace laba_1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            textBox1.Text = File.ReadAllText("../../Source_code.txt");
            listBox1.Items.AddRange(File.ReadAllLines("../../Words.txt"));
            listBox2.Items.AddRange(File.ReadAllLines("../../Separators.txt"));
            listBox3.Items.AddRange(File.ReadAllLines("../../Operators.txt"));
        }

        // Анализ
        private void button1_Click(object sender, EventArgs e)
        {
            // Очистить файл числовых констант
            StreamWriter wr = new StreamWriter("../../Numbers.txt");
            wr.Close();
            // Очистить файл идентификаторов
            wr = new StreamWriter("../../Identificators.txt");
            wr.Close();
            // Очистить файл стрковых констант
            wr = new StreamWriter("../../Symbols.txt");
            wr.Close();

            listBox4.Items.Clear();
            listBox5.Items.Clear();
            listBox6.Items.Clear();

            textBox2.Text = "";
            textBox3.Text = "";
            // считать код программы
            List<string> lines = File.ReadAllLines("../../Source_code.txt").ToList<string>();
            // считать таблицу переходов
            List<EnterData> rlines = DataBase.getDataEnterList("transitions.csv");
            // считать служебные слова
            List<string> words = File.ReadAllLines("../../Words.txt").ToList<string>();
            // массив переходов
            string[,] ArrayLines = new string[21, 17];
            for (int i = 0; i < 21; i++)
            {
                ArrayLines[i, 0] = rlines[i].R;
                ArrayLines[i, 1] = rlines[i].E;
                ArrayLines[i, 2] = rlines[i].M;
                ArrayLines[i, 3] = rlines[i].e;
                ArrayLines[i, 4] = rlines[i].space;
                ArrayLines[i, 5] = rlines[i].LeftAngleBracket;
                ArrayLines[i, 6] = rlines[i].RightAngleBracket;
                ArrayLines[i, 7] = rlines[i].separators;
                ArrayLines[i, 8] = rlines[i].point;
                ArrayLines[i, 9] = rlines[i].operations;
                ArrayLines[i, 10] = rlines[i].PlusOrMinus;
                ArrayLines[i, 11] = rlines[i].equally;
                ArrayLines[i, 12] = rlines[i].letters;
                ArrayLines[i, 13] = rlines[i].digits;
                ArrayLines[i, 14] = rlines[i].DoubleQuotes;
                ArrayLines[i, 15] = rlines[i].eol;
                ArrayLines[i, 16] = rlines[i].eof;
            }

            int rowAnaliz = 0; // анализируемая строка программы
            int row = 0;
            int column = 0;

            
            string state = ""; // состояние
            string procedure = ""; // процедура
            Queue<char> chars = new Queue<char>(); // очередь символов

            // анализ символов
            while (true)
            {
                // просмотреть всю строку программы
                if (column < lines[row].Length)
                {
                    // получить столбец (роль)
                    state = ArrayLines[rowAnaliz, AnalizLex(lines[row][column])].ToString();
                    // если найдена запятая - выделить процедуру и состояние
                    if (state.Contains(',')) 
                    {
                        //MessageBox.Show("Найдена запятая");
                        for (int i = 0; i < state.Length; i++)
                        {
                            if (state[i] == ',')
                            {
                                string newState = state.Substring(0, i);
                                procedure = state.Substring(i + 2);
                                state = newState;
                                break;
                            }
                        }
                        // составить слово из набора символов
                        
                        string word = "";
                        int count = chars.Count;

                        for (int i = 0; i < count; i++)
                        {
                            word += chars.Dequeue();
                        }
                        // выполнение процедуры с передачей слова и таблицы слов
                        Procedures(procedure, word, words);
                    }
                    chars.Enqueue(lines[row][column]); // добавить символ в очередь

                    // переход в следующее состояние

                    // если состояние не равно F и Z 
                    if (String.Compare(state, "F") != 0 && String.Compare(state, "Z") != 0)
                    {
                        // обрабатывать строку - новое указанное в ячейке состояние
                        rowAnaliz = int.Parse(state);
                    }
                    else 
                    {
                        MessageBox.Show("Появилась ошибка. Появилась " + state + ", в строке программы " + rowAnaliz + ", на символе " + lines[row][column] + " " + row + " " + column);
                        return;
                    }

                    state = "";
                    procedure = "";
                    // переход к следующему символу 
                    column++;
                }
                // если строка закончилась
                else if (row < lines.Count - 1)
                {
                    // получить столбец (роль)
                    state = ArrayLines[rowAnaliz, AnalizLex('$')].ToString();
                    // если найдена запятая - выделить процедуру и состояние
                    if (state.Contains(','))
                    {
                        //MessageBox.Show("Найдена запятая");
                        for (int i = 0; i < state.Length; i++)
                        {
                            if (state[i] == ',')
                            {
                                string newState = state.Substring(0, i);
                                procedure = state.Substring(i + 2);
                                state = newState;
                                break;
                            }
                        }
                        // составить слово из набора символов
                        string word = "";
                        int count = chars.Count;

                        for (int i = 0; i < count; i++)
                        {
                            word += chars.Dequeue();
                        }
                        // выполнение процедуры с передачей слова и таблицы слов
                        Procedures(procedure, word, words);
                    }
                    // если запятая не найдена, то состояние уже выделено

                    // переход в следующее состояние

                    // обрабатывать строку - новое указанное в ячейке состояние
                    if (String.Compare(state, "F") != 0 && String.Compare(state, "Z") != 0)
                    {
                        // обрабатывать строку - новое указанное в ячейке состояние
                        rowAnaliz = int.Parse(state);
                    }

                    row++;
                    column = 0;
                    textBox2.Text += Environment.NewLine;
                }

                // если конец файла
                else
                {
                    // получить столбец (роль)
                    state = ArrayLines[rowAnaliz, AnalizLex('@')].ToString();
                    // если найдена запятая - выделить процедуру и состояние
                    if (state.Contains(','))
                    {
                        //MessageBox.Show("Найдена запятая");
                        for (int i = 0; i < state.Length; i++)
                        {
                            if (state[i] == ',')
                            {
                                string newState = state.Substring(0, i);
                                procedure = state.Substring(i + 2);
                                state = newState;
                                break;
                            }
                        }
                        // составить слово из набора символов
                        string word = "";
                        int count = chars.Count;

                        for (int i = 0; i < count; i++)
                        {
                            word += chars.Dequeue();
                        }
                        // выполнение процедуры с передачей слова и таблицы слов
                        Procedures(procedure, word, words);
                    }
                    // если запятая не найдена, то состояние уже выделено

                    // переход в следующее состояние

                    // обрабатывать строку - новое указанное в ячейке состояние
                    if (String.Compare(state, "F") != 0 && String.Compare(state, "Z") != 0)
                    {
                        // обрабатывать строку - новое указанное в ячейке состояние
                        rowAnaliz = int.Parse(state);
                    }
                    else if (String.Compare(state, "Z") == 0) 
                    {
                        break;
                    }

                    row++;
                    column = 0;
                    textBox2.Text += Environment.NewLine;
                    break;
                }
            }

            // Проверить на наличие As Integer, End If, End Sub и т.п.

            //string Text = textBox2.Text;
            List<string> Text = new List<string>() { textBox2.Text };
            //foreach (var line in Text)
            //{
            //    MessageBox.Show(line);
            //}
            //MessageBox.Show(Text.ToString());
            //for (int i = 0; i < Text.Count; i++)
            //{

            //}
            // перезаписать в файл
            File.WriteAllText("../../Analiz.txt", textBox2.Text);
            MessageBox.Show("Анализ лексем сохранен в отдельный файл");
            button3.Enabled = true;
        }

        // анализ лексемы, вернуть столбец (роль)
        public int AnalizLex(char l)
        {
            switch (l)
            {
                case 'R': return 0;
                case 'E': return 1;
                case 'M': return 2;
                case 'e': return 3;
                case ' ': return 4;
                case '<': return 5;
                case '>': return 6;
                case '{':
                case '}':
                case ',': return 7;
                case '.': return 8;
                case '*':
                case '/':
                case ')':
                case '(':
                case '&':
                case '^': return 9;
                case '+':
                case '-': return 10;
                case '=': return 11;
                case '"': return 14;
                case '$': return 15; // конец строки
                case '@': return 16; // конец файла
                default:
                    if (char.IsLetter(l)) // если буква
                        return 12;
                    else if (char.IsDigit(l)) // если цифра
                        return 13;
                    break;
            }
            MessageBox.Show("Что-то пошло не так в анализе лексемы. Скорее всего символ \"" + l + "\" не найден");
            return -1; // ошибка 
        }

        // анализ процедуры
        public void Procedures(string proc, string w, List<string> words)
        {
            switch (proc)
            {
                case "P1": // если идентификатор
                    {
                        // считать идентификаторы из файла
                        List<string> identificators = File.ReadAllLines("../../Identificators.txt").ToList<string>();
                        int i = 0;
                        foreach (var identificator in identificators)
                        {
                            i++;
                            if (String.Compare(w.ToLower(), identificator.ToLower()) == 0)
                            {
                                textBox2.Text += "I" + i + " ";
                                return;
                            }
                        }
                        i++;
                        // если числа нет в таблице, то внести его
                        textBox2.Text += "I" + i + " ";
                        listBox5.Items.Add(w);
                        identificators.Add(w);
                        // записать в файл
                        File.WriteAllLines("../../Identificators.txt", identificators);
                        return;
                    }
                case "P2": // если служебное слово
                    {
                        // найти служебное слово в таблице
                        int i = 0;
                        foreach (var word in words)
                        {
                            i++;
                            if (String.Compare(w.ToLower(), word.ToLower()) == 0)
                            {
                                textBox2.Text += "W" + i + " ";
                                return;
                            }
                        }

                        // иначе это идентификатор

                        // считать идентификаторы из файла
                        List<string> identificators = File.ReadAllLines("../../Identificators.txt").ToList<string>();
                        i = 0;
                        foreach (var identificator in identificators)
                        {
                            i++;
                            if (String.Compare(w.ToLower(), identificator.ToLower()) == 0)
                            {
                                textBox2.Text += "I" + i + " ";
                                return;
                            }
                        }
                        i++;
                        // если числа нет в таблице, то внести его
                        textBox2.Text += "I" + i + " ";
                        listBox5.Items.Add(w);
                        identificators.Add(w);
                        // записать в файл
                        File.WriteAllLines("../../Identificators.txt", identificators);
                        return;
                    }
                case "P3": // если число
                    {
                        // считать все числовые константы из файла
                        List<string> numbers = File.ReadAllLines("../../Numbers.txt").ToList<string>();
                        int i = 0;
                        foreach (var number in numbers)
                        {
                            i++;
                            if (String.Compare(w, number) == 0) 
                            {
                                textBox2.Text += "N" + i + " ";
                                return;
                            }
                        }
                        i++;
                        // если числа нет в таблице, то внести его
                        textBox2.Text += "N" + i + " ";
                        listBox4.Items.Add(w);
                        numbers.Add(w);
                        // записать в файл
                        File.WriteAllLines("../../Numbers.txt", numbers);
                        return;
                    }
                case "P4": // если разделитель
                    {
                        // считать все разделители из файла
                        List<string> separators = File.ReadAllLines("../../Separators.txt").ToList<string>();
                        int i = 0;
                        foreach (var separator in separators)
                        {
                            i++;
                            if (String.Compare(w, separator) == 0)
                            {
                                textBox2.Text += "R" + i + " ";
                                return;
                            }
                        }
                        return;
                    }
                case "P5": // если операция
                    {
                        // считать все числовые константы из файла
                        List<string> operators = File.ReadAllLines("../../Operators.txt").ToList<string>();
                        int i = 0;
                        foreach (var oper in operators)
                        {
                            i++;
                            if (String.Compare(w, oper) == 0)
                            {
                                textBox2.Text += "O" + i + " ";
                                return;
                            }
                        }
                        return;
                    }
                case "P6": // если строковая константа
                    {
                        // считать идентификаторы из файла
                        List<string> symbols = File.ReadAllLines("../../Symbols.txt").ToList<string>();
                        int i = 0;
                        w = w.Trim('"');
                        foreach (var symbol in symbols)
                        {
                            i++;
                            if (String.Compare(w, symbol) == 0)
                            {
                                textBox2.Text += "C" + i + " ";
                                return;
                            }
                        }
                        i++;
                        // если числа нет в таблице, то внести его
                        textBox2.Text += "C" + i + " ";
                        listBox6.Items.Add(w);
                        symbols.Add(w);
                        // записать в файл
                        File.WriteAllLines("../../Symbols.txt", symbols);
                        return;
                    }
                case "P7": return; // проигнорировать цепочку
            }
            MessageBox.Show("Что-то пошло не так");
            return;
        }

        // Изменение файла
        private void button2_Click(object sender, EventArgs e)
        {
            // перезаписать в файл
            File.WriteAllText("../../Source_code.txt", textBox1.Text);
            MessageBox.Show("Изменения успешно сохранены");
        }

        // ОПЗ
        private void button3_Click(object sender, EventArgs e)
        {
            Stack<string> stack = new Stack<string>();

            int MIf = 1;
            textBox3.Text = "";
            // считать результат лексического анализа
            List<string> lines = File.ReadAllLines("../../Analiz.txt").ToList<string>();

            Queue<char> chars = new Queue<char>(); // набор считанных символов
            
            // считать таблицу переходов
            List<Element> rlines = DataBase1.getDataEnterList("table.csv");

            // массив переходов
            string[,] ArrayLines = new string[11, 14];
            for (int i = 0; i < 11; i++)
            {
                ArrayLines[i, 0] = rlines[i].LeftBracket;
                ArrayLines[i, 1] = rlines[i].RightBracket;
                ArrayLines[i, 2] = rlines[i].Сomma;
                ArrayLines[i, 3] = rlines[i].CondIf;
                ArrayLines[i, 4] = rlines[i].CondThen;
                ArrayLines[i, 5] = rlines[i].CondElse;
                ArrayLines[i, 6] = rlines[i].Dim;
                ArrayLines[i, 7] = rlines[i].Type;
                ArrayLines[i, 8] = rlines[i].Equal;
                ArrayLines[i, 9] = rlines[i].Op3;
                ArrayLines[i, 10] = rlines[i].Op4;
                ArrayLines[i, 11] = rlines[i].Op5;
                ArrayLines[i, 12] = rlines[i].Op6;
                ArrayLines[i, 13] = rlines[i].Op7;
            }
            
            int row = 0; // строка в таблице
            int column = 0; // столбец в таблице

            foreach (var line in lines) // для каждой строки анализа
            {
                for (int c = 0; c < line.Length; c++) // для каждого символа
                {
                    if (line[c] != ' ') // если не пробел, то заносим в очередь
                        chars.Enqueue(line[c]);
                    else // появился пробел, составляем слово
                    {
                        string word = ""; // слово
                        int count = chars.Count; // запомнить количество символов в очереди
                        for (int i = 0; i < count; i++) // составить слово
                            word += chars.Dequeue();
                        if (GetPriority(word) == -1 && word != "R1" && word != "R2") 
                            textBox3.Text += word + " "; // если это не операция, то просто вывести
                        else if (word == "R1" || word == "R2")
                            word = "";
                        else
                        {
                            if (stack.Count == 0)  // если стек пуст, операция заносится в стек
                                                   //stack.Push(word);
                            {
                                row = 0;
                                column = GetColumn(word);

                                // выполнить то, что находится на пересечении
                                if (ArrayLines[row, column] == "Err")
                                    MessageBox.Show("Появилась ошибка");
                                else
                                {
                                    MakeProc(ArrayLines[row, column], word, stack, ref MIf);
                                }
                            }
                            else if (stack.Count != 0 && GetColumn(word) != -1) // если стек не пуст
                            {
                                string Last = stack.Peek(); // последний элемент стека
                                row = GetRow(Last); // получить строку в таблице
                                column = GetColumn(word); // получить столбец в таблице переходов

                                // выполнить то, что находится на пересечении
                                if (ArrayLines[row, column] == "Err")
                                    MessageBox.Show("Появилась ошибка");
                                else
                                {
                                    MakeProc(ArrayLines[row, column], word, stack, ref MIf);
                                }
                            }
                        }
                    }
                }

                string w = ""; // слово
                int calc = chars.Count; // запомнить количество символов в очереди
                for (int i = 0; i < calc; i++) // составить слово
                    w += chars.Dequeue();
                textBox3.Text += w + " "/*Environment.NewLine*/;
                while (stack.Count != 0) 
                {
                    if (stack.Peek()=="M1 W8" || stack.Peek() == "M2 W8" || stack.Peek() == "M3 W8" || stack.Peek() == "M4 W8")
                    {
                        string Cond = stack.Pop();
                        Cond = Cond.Substring(0, Cond.Length - 3);
                        textBox3.Text += Cond;
                    }
                    else
                        textBox3.Text += stack.Pop() + " ";
                }
                textBox3.Text += Environment.NewLine;
            }

            // перезаписать в файл
            File.WriteAllText("../../OPZ.txt", textBox3.Text);
            MessageBox.Show("ОПЗ сохранена в отдельный файл");
        }
        
        // Таблица приоритетов операций для алгоритма Дейкстры 
        public int GetPriority(string lex)
        {
            switch (lex)
            {
                case "W8":              // If
                case "O12": return 0;   // (
                case "R3":
                case "R4":
                case "R2":              // ,
                case "O13":             // )
                case "W9":              // Then
                case "W10": return 1;   // Else
                case "W5": return 2;    // GoTo
                case "O14": return 3;   // &
                case "O8":              // =
                case "O6":              // <
                case "O7":              // >
                case "O9":              // <>
                case "O11":             // <=
                case "O10": return 4;   // >=
                case "O1":              // +
                case "O2": return 5;    // -
                case "O3":              // *
                case "O4": return 6;    // /
                case "O5": return 7;    // ^
                case "W16":             // Sub
                case "W4": return 8;         // Dim
                                              //case "W4":     // Dim
            }
            return -1; // не операция 
        }
        
        // Выполнить процедуру
        public void MakeProc(string cell, string word, Stack<string> stack, ref int MIf)
        {
            // если есть запятая в ячейке, значит нужно выполнить несколько процедур

            string[] operations = new string[3];
            if (cell.Contains(',')) 
            {
                operations = cell.Split(new char[] { ',' });
            }
            else
            {
                operations[0]=cell;
            }

            foreach (var op in operations)
            {
                switch (op)
                {
                    case "Вт":
                        // поместить элемент в вершину стека
                        {
                            stack.Push(word);
                            break;
                        }
                    case "Вт(If)":
                        {
                            stack.Push(word);
                            break;
                        }
                    case "Вт(i,j,1 D)":
                        {
                            break;
                        }
                    case "Выт":
                        // поместить элемент в выходную строку
                        {
                            stack.Pop();
                            break;
                        }
                    case "Вых(M УПЛ)":
                        // вывести в выходную строку
                        {
                            textBox3.Text += "M" + MIf + " УПЛ ";
                            //MIf++;
                            break;
                        }
                    case "Вых(M+1 БП M:)":
                        // вывести в выходную строку
                        {
                            MIf++;
                            textBox3.Text += "M" + MIf + " БП M" + (MIf - 1) + ": ";
                            break;
                        }
                    case "Зам(M If)":
                        // заменить элемент в вершине стека на M If
                        {
                            stack.Pop();
                            stack.Push("M" + MIf + " W8");
                            //MIf++;
                            break;
                        }
                    case "Зам(M+1 If)":
                        // заменить элемент в вершмне стека на M+1 If
                        {
                            stack.Pop();
                            stack.Pop();
                            stack.Push("M" + MIf + " W8");
                            break;
                        }
                    case "Вых(M+1:)":
                        // вывести в выходную строку
                        {
                            MIf++;
                            textBox3.Text += "M" + MIf + ": ";
                            break;
                        }
                    case "Држ":
                        // не переходить к новому символу входной строки 
                        {
                            if (word != "O13") 
                                stack.Push(word);
                            break;
                        }
                    case "Вых":
                        // поместить элемент в выходную строку
                        {
                            textBox3.Text += stack.Pop() + " ";
                            while (stack.Count != 0) 
                            {
                                if (GetPriority(stack.Peek()) > GetPriority(word))
                                {
                                    textBox3.Text += stack.Pop() + " ";
                                }
                                else
                                    break;
                            }
                            break;
                        }
                    default:
                        break;
                }
            }
        }

        // получить строку в таблице переходов
        public int GetRow(string lex)
        {
            switch (lex)
            {
                case "O12": return 1;   // (
                case "W8":  return 2; // If
                case "M1 W8":         // M1 If
                case "M2 W8":         // M2 If
                case "M3 W8":         // M3 If
                case "M4 W8": return 3; // M4 If ??
                case "O14": return 6;   // &
                case "O8":              // =
                case "O6":              // <
                case "O7":              // >
                case "O9":              // <>
                case "O11":             // <=
                case "O10": return 7;   // >=
                case "O1":              // +
                case "O2": return 8;    // -
                case "O3":              // *
                case "O4": return 9;    // /
                case "O5": return 10;    // ^
                case "W16":             // Sub
                case "W4": return 9;         // Dim
                                              //case "W4":     // Dim
            }
            return -1; // ошибка 
        }

        // получить столбец в таблице переходов
        public int GetColumn(string word)
        {
            switch (word)
            {
                case "O12": return 0;   // (
                case "O13": return 1;   // )
                case "R2": return 2;   // ,
                case "W8": return 3;   // ,
                case "W9": return 4; // Then
                case "W10": return 5;   // Else
                case "O8": return 8;    // =
                case "O14": return 9;   // &  ОП3
                //case "GoTo": return 2;    // GoTo
                case "O6":              // <  ОП4
                case "O7":              // >
                case "O9":              // <>
                case "O11":             // <=
                case "O10": return 10;   // >=
                case "O1":              // +  ОП5
                case "O2": return 11;    // -
                case "O3":              // *  ОП6
                case "O4": return 12;    // / 
                case "O5": return 13;    // ^  ОП7
                case "W16":             // Sub
                case "W4": return 6;         // Dim
                                              //case "W4":     // Dim
            }
            return -1; // это идентификатор или константа  
        }
    }

    // считывание таблицы переходов
    static class DataBase // база данных
    {
        // переместить из csv-файла в лист
        static public List<EnterData> getDataEnterList(string file)
        {
            List<EnterData> getEnterDataList = new List<EnterData>();
            using (StreamReader srDataEnter = new StreamReader(@"../../" + file, Encoding.UTF8)) 
            {
                var csvDataEnter = new CsvReader(srDataEnter);
                csvDataEnter.Configuration.Delimiter = "\t";

                while (csvDataEnter.Read())
                {
                    EnterData oneRecord = new EnterData()
                    {
                       R = csvDataEnter.GetField<string>(0),
                       E = csvDataEnter.GetField<string>(1),
                       M = csvDataEnter.GetField<string>(2),
                       e = csvDataEnter.GetField<string>(3),
                       space = csvDataEnter.GetField<string>(4),
                       LeftAngleBracket = csvDataEnter.GetField<string>(5),
                       RightAngleBracket = csvDataEnter.GetField<string>(6),
                       separators = csvDataEnter.GetField<string>(7),
                       point = csvDataEnter.GetField<string>(8),
                       operations = csvDataEnter.GetField<string>(9),
                       PlusOrMinus = csvDataEnter.GetField<string>(10),
                       equally = csvDataEnter.GetField<string>(11),
                       letters = csvDataEnter.GetField<string>(12),
                       digits = csvDataEnter.GetField<string>(13),
                       DoubleQuotes = csvDataEnter.GetField<string>(14),
                       eol = csvDataEnter.GetField<string>(15),
                       eof = csvDataEnter.GetField<string>(16),
                    };
                    getEnterDataList.Add(oneRecord);

                }
                srDataEnter.Close();
                return getEnterDataList;
            }
        }

        // из лист в csv в конец файла
        static public void setDataEnterList(List<EnterData> setEnterDataList)
        {
            using (var swDataEnter = new StreamWriter(@"../../analiz.csv", true, Encoding.UTF8))
            {
                using (var csvDataEnter = new CsvWriter(swDataEnter))
                {
                    csvDataEnter.Configuration.HasHeaderRecord = false;
                    csvDataEnter.WriteRecords(setEnterDataList);
                }
                swDataEnter.Close();
            }
        }
    }

    // считывание таблицы переходов 1
    static class DataBase1 // база данных
    {
        // переместить из csv-файла в лист
        static public List<Element> getDataEnterList(string file)
        {
            List<Element> getEnterDataList = new List<Element>();
            using (StreamReader srDataEnter = new StreamReader(@"../../" + file, Encoding.UTF8))
            {
                var csvDataEnter = new CsvReader(srDataEnter);
                csvDataEnter.Configuration.Delimiter = "\t";

                while (csvDataEnter.Read())
                {
                    Element oneRecord = new Element()
                    {
                        LeftBracket = csvDataEnter.GetField<string>(0),
                        RightBracket = csvDataEnter.GetField<string>(1),
                        Сomma = csvDataEnter.GetField<string>(2),
                        CondIf = csvDataEnter.GetField<string>(3),
                        CondThen = csvDataEnter.GetField<string>(4),
                        CondElse = csvDataEnter.GetField<string>(5),
                        Dim = csvDataEnter.GetField<string>(6),
                        Type = csvDataEnter.GetField<string>(7),
                        Equal = csvDataEnter.GetField<string>(8),
                        Op3 = csvDataEnter.GetField<string>(9),
                        Op4 = csvDataEnter.GetField<string>(10),
                        Op5 = csvDataEnter.GetField<string>(11),
                        Op6 = csvDataEnter.GetField<string>(12),
                        Op7 = csvDataEnter.GetField<string>(13),
                    };
                    getEnterDataList.Add(oneRecord);

                }
                srDataEnter.Close();
                return getEnterDataList;
            }
        }
    }
    }
