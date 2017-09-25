function formEmployee(employee, maxEmployeeLength) {
    var CUTTED_EMPLOYEE_END = '...';
    var EMPLOYEE_SEPARATOR = ', ';
    var ROW_SEPARATOR = '\n';
    var result = new Array('');
    var j = 0;
    var i = 0;
    var allEmployess = employee.split(',');
    while (i < allEmployess.length) {
        if (result[j].length + allEmployess[i].length < maxEmployeeLength) {
            result[j] += allEmployess[i] + EMPLOYEE_SEPARATOR;
            i++;
        } else if (allEmployess[i].length >= maxEmployeeLength) {
            if (result[j].length == 0) {
                result[j] = allEmployess[i].substr(0, maxEmployeeLength - 1) + CUTTED_EMPLOYEE_END;
                i++;
            } else {
                result.push(allEmployess[i].substr(0, maxEmployeeLength - 1) + CUTTED_EMPLOYEE_END);
                j++;
                i++;
            }
        } else {
            j++;
            result.push('');
        }
    }
    TrimLastSeparator();
    return result.join(ROW_SEPARATOR);

    function TrimLastSeparator() {
        result[result.length - 1] = result[result.length - 1].substr(0, result[result.length - 1].lastIndexOf(EMPLOYEE_SEPARATOR));
    }
}