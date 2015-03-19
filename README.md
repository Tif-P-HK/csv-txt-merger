## Csv/Txt Files Merger
A simple tool that loads multiple csv and/or txt files, merges them into one by picking one line from each source file one at a time, and exports the merged file into a txt file.

###Screenshots
![Add csv/txt files to apps](https://github.com/Tif-P-HK/csv-txt-merger/blob/master/screenshots/Files.jpg)
![Preview a file](https://github.com/Tif-P-HK/csv-txt-merger/blob/master/screenshots/Data.jpg)
![Merge input files into one](https://github.com/Tif-P-HK/csv-txt-merger/blob/master/screenshots/Result.jpg)

### Target users
Those who have some simulation files to be used by the [GeoEvent Extension](http://www.esri.com/software/arcgis/arcgisserver/extensions/geoevent-extension) of [Esri](http://www.esri.com/)'s [ArcGIS for Server](http://www.esri.com/software/arcgis/arcgisserver), and those who simply want to merge a few files into one.

### Pre-requisite
* .Net Framework 4.0 or above

### How to use the app
1. Browse for as many csv/txt files as needed. File header is optional and files don't need to have the same headers (i.e. number of fields and field names can be different).
2. Added files will be listed and they can be previewed. For the file with the greatest number of fields, it will always be at the top of the file list.
3. If a file has more lines than the rest of the files, the extra lines will be omitted during file merge.
4. When all files are in place, merge them into one and the output will be shown on the result page
5. Export the merged file as needed.

### Credit
[MahApps.Metro](http://mahapps.com/)

### License
The MIT License (MIT)

Copyright (c) 2015 Tif-Pun

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

