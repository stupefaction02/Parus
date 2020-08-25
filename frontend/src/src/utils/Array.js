Array.Copy = function(sourceArray, sourceIndex, destinationArray, destinationIndex, length) {
    while(length--) destinationArray[destinationIndex++] = sourceArray[sourceIndex++]; 
};
/*
Array.prototype.indexOfRange = function(needle, startIndex, sourceLength) {
    var sliced = this.slice(startIndex, sourceLength);

    return sliced.indexOf(needle[0], 0);
}
*/
Array.prototype.indexOfRange = function(value, startIndex, count) {
    var sliced = this.slice(startIndex, count);
    return sliced.indexOf(value);
}

Array.prototype.FindSubArray = function(needle, startIndex, sourceLength) {
    var needleLen = needle.length;
    var index;

    while (sourceLength >= needleLen) {
        index = this.indexOfRange(needle[0], startIndex, sourceLength - needleLen + 1);

        if (index == -1)
            return -1;

        var i, p;
        for (i = 0, p = index; i < needleLen; i++, p++)
            if (this[p] != needle[i])
                break;

        if (i == needleLen)
            return index;

        sourceLength -= (index - startIndex + 1);
        startIndex = index + 1;
    }
     
    return -1;
};
