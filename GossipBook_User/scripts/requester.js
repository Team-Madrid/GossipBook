var ajaxRequester = (function () {
   var headers = {
        'X-Parse-Application-Id': 'dYRqdJby7m0aLjdNEOejSOO1yDG2w5zXA9lFOHsz',
        'X-Parse-REST-API-Key': '5ogFu1FVYT2WXnOIreTAXu0VIkvdnHEQgBVaxHP4'
    };
    
	var makeRequest = function (method, url, data, success, error) {
		$.ajax({		
			url: url,
			type: method,
			//dataType: 'jsonp',
			//contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
			//accepts: "application/json",
			data: data || undefined,
			//processData: true,
			success: success,
			error: error
		});
	};

	function makeGetRequest (url, data, success, error) {
		return makeRequest('GET', url, data, success, error);
	};

	function makePostRequest (url, data, success, error) {
		return makeRequest('POST', url, data, success, error);
	};

	function makePutRequest (url, data, success, error) {
		return makeRequest('PUT', url, data, success, error);
	};

	function makeDeleteRequest (url, success, error) {
		return makeRequest('DELETE', url, null, success, error);
	};

	return {
		get: makeGetRequest,
		post: makePostRequest,
		put: makePutRequest,
		delete: makeDeleteRequest
	};
}());