function getTimecodeFragment(documentId, timeSeconds) {
    var context = getContext();
    var collection = context.getCollection();
    var query = "SELECT document.height, document.width, document.timescale, document.framerate, fragment";
    query = query + " FROM document JOIN fragment IN document.fragments";
    query = query + " WHERE document.id = '" + documentId + "'";
    query = query + " AND udf.isTimecodeFragment(" + timeSeconds + ", document.timescale, fragment)";
    var accepted = collection.queryDocuments(collection.getSelfLink(), query,
        function (err, feed, options) {
            if (err) {
                throw err;
            } else if (feed && feed.length) {
                var response = context.getResponse();
                response.setBody(JSON.stringify(feed[0]));
            }
        }
    );
    if (!accepted) {
        throw new Error("The query was not accepted by the server.");
    }
}