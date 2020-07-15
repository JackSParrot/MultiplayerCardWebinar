function doGet(r)
{
  var uid = r.parameter.uid;
  var sheet = SpreadsheetApp.openById('#your-sheet-id-here#').getSheets()[0];
  var cell = sheet.createTextFinder(uid).findNext();
  if(cell != null)
  {
    sheet.deleteRow(cell.getRow()); 
  }
  sheet.appendRow([uid, r.parameter.name, r.parameter.deck]);
  return ContentService.createTextOutput("ok");
}