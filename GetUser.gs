function doGet(r)
{
  var uid = r.parameter.uid;
  var data = SpreadsheetApp.openById('#your-sheet-id-here#').getSheets()[0].getDataRange().getValues();
  var retVal = '';
  while(retVal == '')
  {
    var row =  data[1 + (Math.floor(Math.random() * (data.length-1)))];
    if(uid != row[0] && row[2] != '')
    {
      retVal = row[1]+':'+row[2]
    }
  }
  return ContentService.createTextOutput(retVal);
}