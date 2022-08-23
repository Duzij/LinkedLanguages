import React from "react";

export default function LoadingSpinner(props) {
  if (props.loading) {
  return (
    <div className="overlay d-flex justify-content-center align-items-center">
      <div className="spinner-border text-light" role="status">
        <span className="visually-hidden">Loading...</span>
      </div>
    </div>);
  }
}