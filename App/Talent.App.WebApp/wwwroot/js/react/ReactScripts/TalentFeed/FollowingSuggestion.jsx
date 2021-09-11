import React from "react";

export default class FollowingSuggestion extends React.Component {
  render() {
    const { talents } = this.props;
    return (
      <div className="content">
        <div className="center aligned header">Follow Talent</div>
        <div className="ui items following-suggestion">
          {talents.map((t) => (
            <div key={t.id} className="item">
              <div className="ui image">
                <img
                  className="ui circular image"
                  src={
                    t.photoId
                      ? t.photoId
                      : "http://semantic-ui.com/images/avatar/small/jenny.jpg"
                  }
                />
              </div>
              <div className="content">
                <a className="">{t.name}</a>
                <button className="ui primary basic button">
                  <i className="icon user"></i>Follow
                </button>
              </div>
            </div>
          ))}
        </div>
      </div>
    );
  }
}
