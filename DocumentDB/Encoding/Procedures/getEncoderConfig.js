function getEncoderConfig(propertyName, propertyValue) {
    var context = getContext();
    var collection = context.getCollection();
    var query = "SELECT * FROM document WHERE document." + propertyName + " = '" + propertyValue + "'";
    var accepted = collection.queryDocuments(collection.getSelfLink(), query,
        function (err, feed, options) {
            if (err) throw err;
            var response = context.getResponse();
            if (!feed || !feed.length) {
                response.setBody("No documents found.");
            } else {
                response.setBody(JSON.stringify(feed[0]));
            }
        }
    );
    if (!accepted) {
        throw new Error("The query was not accepted by the server.");
    }
}
