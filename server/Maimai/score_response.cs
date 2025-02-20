namespace Maimai
{
    public class ScoreResponse
    {
        public Score[] data;
        public object links;
        public object meta;
    }
}
/*
"links": {
        "first": "https:\/\/maitea.app\/api\/v1\/plays?page=1",
        "last": "https:\/\/maitea.app\/api\/v1\/plays?page=1",
        "prev": null,
        "next": null
    },
    "meta": {
        "current_page": 1,
        "from": 1,
        "last_page": 1,
        "links": [
            {
                "url": null,
                "label": "&laquo; Previous",
                "active": false
            },
            {
                "url": "https:\/\/maitea.app\/api\/v1\/plays?page=1",
                "label": "1",
                "active": true
            },
            {
                "url": null,
                "label": "Next &raquo;",
                "active": false
            }
        ],
        "path": "https:\/\/maitea.app\/api\/v1\/plays",
        "per_page": 12,
        "to": 8,
        "total": 8
    }*/