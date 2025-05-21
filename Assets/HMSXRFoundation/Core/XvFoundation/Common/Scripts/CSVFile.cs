using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class CSVFile
{
    List<string[]> _data = new List<string[]>();

    /// <summary>
    /// CSV data array
    /// </summary>
    public string[][] Data
    {
        get { return _data.ToArray(); }
    }

    int _rowCount = 0;

    /// <summary>
    /// Number of rows
    /// </summary>
    public int RowCount
    {
        get { return _rowCount; }
    }

    int _colCount = 0;

    /// <summary>
    /// Number of columns
    /// </summary>
    public int ColCount
    {
        get { return _colCount; }
    }

    /// <summary>
    /// Initialize CSV file from byte array with specified encoding
    /// </summary>
    /// <param name="data">CSV file content in byte array</param>
    /// <param name="encoding">Text encoding</param>
    public CSVFile(byte[] data, Encoding encoding)
    {
        var content = encoding.GetString(data);

        string[] rows = content.Split(new string[] { "\r\n" }, StringSplitOptions.None);
        MyDebugTool.LogError("rows:" + rows.Length);

        ParseRows(rows);
    }

    /// <summary>
    /// Initialize CSV file from file path
    /// </summary>
    /// <param name="path">CSV file path</param>
    public CSVFile(string path)
    {
        System.Text.Encoding encoding = GetEncoding(path);
        string[] rows = File.ReadAllLines(path, encoding);
        ParseRows(rows);
    }

    /// <summary>
    /// Parse CSV rows into data structure
    /// </summary>
    /// <param name="rows">Array of CSV rows</param>
    void ParseRows(string[] rows)
    {
        for (int i = 0; i < rows.Length; i++)
        {
            if (string.IsNullOrEmpty(rows[i]))
            {
                continue;
            }
            else
            {
                var cols = GetCols(rows[i]);
                if (null != cols)
                {
                    _data.Add(cols.ToArray());
                }
            }

        }

        _rowCount = _data.Count;
        if (_rowCount > 0)
        {
            _colCount = _data[0].Length;
        }
    }

    /// <summary>
    /// Get cell value at specified row and column
    /// </summary>
    /// <param name="row">Row index</param>
    /// <param name="col">Column index</param>
    /// <returns>Cell value as string</returns>
    public string GetValue(int row, int col)
    {
        return _data[row][col];
    }

    /// <summary>
    /// Get entire row data
    /// </summary>
    /// <param name="row">Row index</param>
    /// <returns>Array of cell values in the row</returns>
    public string[] GetValue(int row)
    {
        string[] tmpStr = new string[ColCount];
        tmpStr[0] = _data[row][0];
        for (int i = 1; i < ColCount; i++)
        {
            tmpStr[i] = (_data[row][i]);
        }
        return tmpStr;
    }

    /// <summary>
    /// Split CSV row into columns, handling quotes and commas within quotes
    /// </summary>
    /// <param name="rowContent">CSV row string</param>
    /// <returns>List of column values</returns>
    List<string> GetCols(string rowContent)
    {
        // Quotation mark character
        const char QUOTATION_MARKS = '"';
        // Comma character
        const char COMMA = ',';

        List<string> cols = new List<string>();
        // Split marker (also the index of the first character of a column string)
        int splitMark = 0;
        int charIdx = 0;
        bool isSpecial = false;

        while (charIdx < rowContent.Length)
        {
            char c = rowContent[charIdx];
            int nextIdx = charIdx + 1;

            if (charIdx == splitMark)
            {
                if (c == QUOTATION_MARKS)
                {
                    isSpecial = true;
                }
                else
                {
                    isSpecial = false;
                    if (nextIdx == rowContent.Length)
                    {
                        // End of row
                        string colContent = rowContent.Substring(splitMark);
                        cols.Add(colContent);
                        break;
                    }
                }
            }
            else
            {
                if (isSpecial)
                {
                    // Handle quoted values
                    if (c == QUOTATION_MARKS)
                    {
                        if (nextIdx == rowContent.Length)
                        {
                            // End of row
                            string colContent = rowContent.Substring(splitMark + 1, charIdx - splitMark - 1);
                            colContent = colContent.Replace("\"\"", "\"");
                            cols.Add(colContent);
                            // Skip next character
                            charIdx++;
                        }
                        else
                        {
                            char nextC = rowContent[nextIdx];
                            if (nextC == QUOTATION_MARKS)
                            {
                                // Skip escaped quote
                                charIdx++;
                            }
                            else if (nextC == COMMA)
                            {
                                // Comma after quote - end of column
                                string colContent = rowContent.Substring(splitMark + 1, charIdx - splitMark - 1);
                                colContent = colContent.Replace("\"\"", "\"");
                                cols.Add(colContent);
                                charIdx++;
                                splitMark = nextIdx + 1;
                            }
                        }
                    }
                }
                else
                {
                    // Handle normal values
                    if (c == COMMA)
                    {
                        // Comma - end of column
                        string colContent = rowContent.Substring(splitMark, charIdx - splitMark);
                        cols.Add(colContent);
                        splitMark = charIdx + 1;
                    }

                    if (nextIdx == rowContent.Length)
                    {
                        // End of row
                        string colContent = rowContent.Substring(splitMark);
                        cols.Add(colContent);
                        break;
                    }
                }
            }

            charIdx++;
        }

        return cols;
    }

    /// <summary>
    /// Detect file encoding from file path
    /// </summary>
    /// <param name="FILE_NAME">File path</param>
    /// <returns>Detected encoding</returns>
    System.Text.Encoding GetEncoding(string FILE_NAME)
    {
        FileStream fs = new FileStream(FILE_NAME, FileMode.Open, FileAccess.Read);
        System.Text.Encoding r = GetEncoding(fs);
        fs.Close();
        return r;
    }

    /// <summary>
    /// Detect file encoding from file stream by checking BOM
    /// </summary>
    /// <param name="fs">File stream</param>
    /// <returns>Detected encoding</returns>
    System.Text.Encoding GetEncoding(FileStream fs)
    {
        BinaryReader r = new BinaryReader(fs, System.Text.Encoding.Default);
        byte[] ss = r.ReadBytes(3);
        r.Close();

        if (ss[0] >= 0xEF)
        {
            if (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF)
            {
                return System.Text.Encoding.UTF8;
            }
            else if (ss[0] == 0xFE && ss[1] == 0xFF)
            {
                return System.Text.Encoding.BigEndianUnicode;
            }
            else if (ss[0] == 0xFF && ss[1] == 0xFE)
            {
                return System.Text.Encoding.Unicode;
            }
            else
            {
                return System.Text.Encoding.Default;
            }
        }
        else
        {
            return System.Text.Encoding.Default;
        }
    }
}