import React, { Fragment } from "react";
import ReactPlayer from "react-player";
import PropTypes from "prop-types";
import TalentCardDetail from "../TalentFeed/TalentCardDetail.jsx";

export default class TalentCard extends React.Component {
  constructor(props) {
    super(props);
  }

  render() {
    const { profileDate } = this.props;
    console.log("Talent Card :", profileDate);
    return (
      <React.Fragment>
        {profileDate ? (
          profileDate.map((t) => (
            <div key={t.id}>
              <TalentCardDetail profileDate={t} />
              <br />
            </div>
          ))
        ) : (
          <div>There are no talents found for your recruitment company</div>
        )}
      </React.Fragment>
    );
  }
}
