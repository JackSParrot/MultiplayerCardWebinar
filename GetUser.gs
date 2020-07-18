function doGet(r)
{
  var uid = r.parameter.uid;
  var data = SpreadsheetApp.openById('#your-sheet-id-here#').getSheets()[0].getDataRange().getValues();
  var retVal = '';
  var timeout = 10;
  while(timeout > 0)
  {
    var row =  data[1 + (Math.floor(Math.random() * (data.length-1)))];
    if(uid != row[0] && row[2] != '')
    {
      retVal = row[1]+':'+row[2];
      timeout = 0;
    }
    --timeout;
  }
  return ContentService.createTextOutput(retVal);
}