import React from "react";
import ReactDOM from "react-dom";
import ReactPlayer from "react-player";
import {
  Icon,
  Grid,
  Card,
  Image,
  Label,
  List,
  Header,
} from "semantic-ui-react";

export default class TalentCardDetail extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      showDetails: false,
    };
    this.displayDetails = this.displayDetails.bind(this);
    this.displayCard = this.displayCard.bind(this);
  }

  displayDetails() {
    this.setState({
      showDetails: true,
    });
  }

  displayCard() {
    this.setState({
      showDetails: false,
    });
  }

  render() {
    const { profileDate } = this.props;
    console.log("Talent Details :", profileDate);
    if (this.state.showDetails === true) {
      return (
        <Card fluid>
          <Card.Content>
            <Header as="h2" floated="right">
              <Icon name="star" size="big" />
            </Header>
            <Header as="h2" floated="left">
              {profileDate.name}
            </Header>
          </Card.Content>
          <Card.Content>
            <Grid divided="vertically">
              <Grid.Row columns={2}>
                <Grid.Column>
                  <Image
                    size="medium"
                    floated="left"
                    src={
                      profileDate.photoId
                        ? profileDate.photoId
                        : "https://react.semantic-ui.com/images/avatar/large/matthew.png"
                    }
                  />
                </Grid.Column>
                <Grid.Column>
                  <Header as="h4">{profileDate.name}</Header>

                  <List>
                    <List.Item>
                      <List.Header>CURRENT EMPLOYER</List.Header>
                      {profileDate.currentEmployment}
                    </List.Item>
                    <List.Item>
                      <List.Header>VISA STATUS</List.Header>
                      {profileDate.visa ? profileDate.visa : "No Visa Status"}
                    </List.Item>
                    <List.Item>
                      <List.Header>POSITION</List.Header>
                      {profileDate.level}
                    </List.Item>
                  </List>
                </Grid.Column>
              </Grid.Row>
            </Grid>
          </Card.Content>
          <Card.Content>
            <Grid textAlign="center" reversed="mobile" columns="equal">
              <Grid.Column>
                <a onClick={this.displayCard}>
                  <Icon name="video camera" size="big" />
                </a>
              </Grid.Column>
              <Grid.Column>
                <Icon name="file pdf outline" size="big" />
              </Grid.Column>
              <Grid.Column>
                <a
                  onClick={() =>
                    profileDate.linkedIn
                      ? window.open(`${profileDate.linkedin}`)
                      : window.open("https://www.linkedin.com/")
                  }
                >
                  <Icon name="linkedin" size="big" />
                </a>
              </Grid.Column>
              <Grid.Column>
                <a
                  onClick={() =>
                    profileDate.gitHub
                      ? window.open(`${profileDate.gitHub}`)
                      : window.open("https://www.github.com/")
                  }
                >
                  <Icon link name="github" size="big" />
                </a>
              </Grid.Column>
            </Grid>
          </Card.Content>
          <Card.Content extra>
            {profileDate.skills ? (
              profileDate.skills.map((s) => (
                <Label key={s} as="a" basic color="blue">
                  {s}
                </Label>
              ))
            ) : (
              <Label as="a" basic color="blue">
                No Skills
              </Label>
            )}
          </Card.Content>
        </Card>
      );
    } else {
      return (
        <Card fluid>
          <Card.Content>
            <Header as="h2" floated="right">
              <Icon name="star" size="big" />
            </Header>
            <Header as="h2" floated="left">
              {profileDate.name}
            </Header>
          </Card.Content>
          <ReactPlayer width="550px" url="https://youtu.be/D_LqhNqKQQQ" />
          <Card.Content>
            <Grid textAlign="center" reversed="mobile" columns="equal">
              <Grid.Column>
                <a onClick={this.displayDetails}>
                  <Icon name="user" size="big" />
                </a>
              </Grid.Column>
              <Grid.Column>
                <Icon name="file pdf outline" size="big" />
              </Grid.Column>
              <Grid.Column>
                <a
                  onClick={() =>
                    profileDate.linkedIn
                      ? window.open(`${profileDate.linkedin}`)
                      : window.open("https://www.linkedin.com/")
                  }
                >
                  <Icon name="linkedin" size="big" />
                </a>
              </Grid.Column>
              <Grid.Column>
                <a
                  onClick={() =>
                    profileDate.gitHub
                      ? window.open(`${profileDate.gitHub}`)
                      : window.open("https://www.github.com/")
                  }
                >
                  <Icon link name="github" size="big" />
                </a>
              </Grid.Column>
            </Grid>
          </Card.Content>
          <Card.Content extra>
            {profileDate.skills ? (
              profileDate.skills.map((s) => (
                <Label key={s} as="a" basic color="blue">
                  {s}
                </Label>
              ))
            ) : (
              <Label as="a" basic color="blue">
                No Skills
              </Label>
            )}
          </Card.Content>
        </Card>
      );
    }
  }
}
